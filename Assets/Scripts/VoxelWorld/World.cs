using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VoxelWorld
{
    public class World : MonoBehaviour
    {
        public GameObject player;
        public Material textureAtlas;
        public static ConcurrentDictionary<string, Chunk> chunks;
        public bool initialBuild = true;
        public static List<string> chunksToRemove = new List<string>();
        public Vector3 lastBuildPosition;

        public CoroutineQueue coroutineQueue;
        public static uint maxCoroutines = 1000;

        [SerializeField] private Slider slider;
        [SerializeField] private Camera loadingCamera;
        [SerializeField] private TextMeshProUGUI statusText;


        private void Start()
        {
            player.SetActive(false);
            Vector3 playerPosition = player.transform.position;
            player.transform.position = new Vector3(playerPosition.x,
                Utils.GenerateHeight(playerPosition.x, playerPosition.z) + 15, playerPosition.z);
            lastBuildPosition = playerPosition;

            initialBuild = true;
            chunks = new ConcurrentDictionary<string, Chunk>();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;


            coroutineQueue = new CoroutineQueue(maxCoroutines, StartCoroutine);

            BuildChunkAt((int) (playerPosition.x / Settings.CHUNK_SIZE), (int) (playerPosition.y / Settings.CHUNK_SIZE),
                (int) (playerPosition.z / Settings.CHUNK_SIZE));

            coroutineQueue.Run(DrawChunks());

            coroutineQueue.Run(BuildWorldRecursively(
                (int) (playerPosition.x / Settings.CHUNK_SIZE),
                (int) (playerPosition.y / Settings.CHUNK_SIZE),
                (int) (playerPosition.z / Settings.CHUNK_SIZE),
                Settings.RENDER_DISTANCE));
            
            
            InvokeRepeating(nameof(SaveChangedChunks), Settings.AUTOSAVE_DELAY, Settings.AUTOSAVE_DELAY);
        }

        public void StartLoading()
        {
            slider.gameObject.SetActive(true);
            statusText.gameObject.SetActive(true);
        }

        private void Update()
        {
            Vector3 movement = lastBuildPosition - player.transform.position;

            if (movement.magnitude > Settings.CHUNK_SIZE)
            {
                lastBuildPosition = player.transform.position;
                BuildNearPlayer();
            }


            if (!player.activeSelf && initialBuild)
            {
                player.SetActive(true);
                initialBuild = false;
            }

            coroutineQueue.Run(DrawChunks());
            coroutineQueue.Run(RemoveOldChunksOutsideRadius());
        }

        public static string BuildChunkName(Vector3 position)
        {
            return (int) position.x + "_" + (int) position.y + "_" + (int) position.z;
        }


        public void BuildChunkAt(int x, int y, int z)
        {
            Vector3 chunkPosition = new Vector3(x * Settings.CHUNK_SIZE, y * Settings.CHUNK_SIZE, z * Settings.CHUNK_SIZE);
            string chunkName = BuildChunkName(chunkPosition);
            Chunk chunk;

            if (!chunks.TryGetValue(chunkName, out chunk))
            {
                chunk = new Chunk(chunkPosition, textureAtlas);
                chunk.chunk.transform.parent = transform;
                chunks.TryAdd(chunk.chunk.name, chunk);
            }
        }

        public IEnumerator BuildWorldRecursively(int x, int y, int z, int radius)
        {
            radius -= 1;
            if (radius <= 0 || y < 0 || y >= 16) yield break;

            BuildChunkAt(x, y, z - 1);
            coroutineQueue.Run(BuildWorldRecursively(x, y, z - 1, radius));

            BuildChunkAt(x, y, z + 1);
            coroutineQueue.Run(BuildWorldRecursively(x, y, z + 1, radius));

            BuildChunkAt(x, y - 1, z);
            coroutineQueue.Run(BuildWorldRecursively(x, y - 1, z, radius));

            BuildChunkAt(x, y + 1, z);
            coroutineQueue.Run(BuildWorldRecursively(x, y + 1, z, radius));

            BuildChunkAt(x - 1, y, z);
            coroutineQueue.Run(BuildWorldRecursively(x - 1, y, z, radius));

            BuildChunkAt(x + 1, y, z);
            coroutineQueue.Run(BuildWorldRecursively(x + 1, y, z, radius));
            yield return null;
        }

        public IEnumerator DrawChunks()
        {
            foreach (KeyValuePair<string, Chunk> chunk in chunks)
            {
                if (chunk.Value.chunkState == ChunkState.Draw)
                {
                    chunk.Value.DrawChunk();
                }

                if (chunk.Value.chunk &&
                    Vector3.Distance(player.transform.position, chunk.Value.chunk.transform.position) >
                    Settings.RENDER_DISTANCE * Settings.CHUNK_SIZE)
                {
                    chunksToRemove.Add(chunk.Key);
                }

                yield return null;
            }
        }

        public IEnumerator RemoveOldChunksOutsideRadius()
        {
            for (int index = 0; index < chunksToRemove.Count; index++)
            {
                string chunkName = chunksToRemove[index];
                if (chunks.TryGetValue(chunkName, out Chunk chunk))
                {
                    Destroy(chunk.chunk);
                    chunk.Save();
                    chunks.TryRemove(chunkName, out chunk);
                    yield return null;
                }
            }
        }

        public void BuildNearPlayer()
        {
            Vector3 playerPosition = player.transform.position;

            StopCoroutine(nameof(BuildWorldRecursively));
            coroutineQueue.Run(BuildWorldRecursively(
                (int) (playerPosition.x / Settings.CHUNK_SIZE),
                (int) (playerPosition.y / Settings.CHUNK_SIZE),
                (int) (playerPosition.z / Settings.CHUNK_SIZE),
                Settings.RENDER_DISTANCE));
        }

        public static Block GetWorldBlock(Vector3 position, Chunk hitChunk)
        {
            Debug.Log("Position: " + position);
            int modX = (int) (position.x % Settings.CHUNK_SIZE);
            int modY = (int) (position.y % Settings.CHUNK_SIZE);
            int modZ = (int) (position.z % Settings.CHUNK_SIZE);

            Debug.Log("Mods: " + modX + " " + modY + " " + modZ);

            int blockX = (int) (Mathf.Floor(position.x) % Settings.CHUNK_SIZE) - (position.x < 0 ? 1 : 0);
            int blockY = (int) (Mathf.Floor(position.y) % Settings.CHUNK_SIZE) - (position.y < 0 ? 1 : 0);
            int blockZ = (int) (Mathf.Floor(position.z) % Settings.CHUNK_SIZE) - (position.z < 0 ? 1 : 0);

            Debug.Log("Blocks: " + blockX + " " + blockY + " " + blockZ);

            int chunkX = (int) Mathf.Floor((int) position.x - modX);
            int chunkY = (int) Mathf.Floor((int) position.y - modY);
            int chunkZ = (int) Mathf.Floor((int) position.z - modZ);

            if (blockX < 0)
            {
                blockX += Settings.CHUNK_SIZE + 1;
                chunkX -= Settings.CHUNK_SIZE;
            }
            
            if (blockZ < 0)
            {
                blockZ += Settings.CHUNK_SIZE + 1;
                chunkZ -= Settings.CHUNK_SIZE;
            }

            
            
            
            Vector3 chunkPosition = new Vector3(chunkX, chunkY, chunkZ);

            string chunkName = BuildChunkName(chunkPosition);
            Debug.Log("Name: " + chunkName);
            Debug.Log("Hit: " + hitChunk.chunkName);
            if (chunks.TryGetValue(chunkName, out Chunk chunk))
            {
                return chunk.chunkData[blockX, blockY, blockZ];
            }
            else return null;

            /*int chunkX = (int) ((float)Mathf.Round(position.x - (position.x < 0 ? chunkSize : 0)) / (float)chunkSize) * chunkSize;
            int chunkY = (int) ((float)Mathf.Round(position.y - (position.y < 0 ? chunkSize : 0)) / (float)chunkSize) * chunkSize;
            int chunkZ = (int) ((float)Mathf.Round(position.z - (position.z < 0 ? chunkSize : 0)) / (float)chunkSize) * chunkSize;

            int blockX = (int) Mathf.Abs(Mathf.Round(position.x) - chunkX);
            int blockY = (int) Mathf.Abs(Mathf.Round(position.y) - chunkY);
            int blockZ = (int) Mathf.Abs(Mathf.Round(position.z) - chunkZ);

            string chunkName = BuildChunkName(new Vector3(chunkX, chunkY, chunkZ));

            if (chunks.TryGetValue(chunkName, out Chunk chunk))
            {
                return chunk.chunkData[blockX, blockY, blockZ];
            } 
            return null;*/
        }

        public IEnumerator SaveChangedChunks()
        {
            foreach (KeyValuePair<string, Chunk> chunkPair in chunks)
            {
                if (chunkPair.Value.changed) chunkPair.Value.Save();
            }

            yield return null;
        }
    }
}