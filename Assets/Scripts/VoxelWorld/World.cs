using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VoxelWorld
{
    public class World : MonoBehaviour
    {
        public GameObject player;
        public Material textureAtlas;
        public Material fluidTextureAtlas;
        public static ConcurrentDictionary<string, Chunk> chunks;
        public bool initialBuild = true;
        private static readonly List<string> ChunksToRemove = new List<string>();
        public Vector3 lastBuildPosition;
        public static CoroutineQueue coroutineQueue;

        public static World instance;

        private void Awake()
        {
            instance = this;
        }

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


            coroutineQueue = new CoroutineQueue(Settings.MAX_COROUTINES, StartCoroutine);

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
                chunk = new Chunk(chunkPosition, textureAtlas, fluidTextureAtlas);
                chunk.world = this;
                chunk.chunk.transform.parent = transform;
                chunk.fluid.transform.parent = transform;
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
                    ChunksToRemove.Add(chunk.Key);
                }

                yield return null;
            }
        }

        public IEnumerator RemoveOldChunksOutsideRadius()
        {
            for (int index = 0; index < ChunksToRemove.Count; index++)
            {
                string chunkName = ChunksToRemove[index];
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

        public static Block GetWorldBlock(Vector3 position)
        {
            int modX = (int) (position.x % Settings.CHUNK_SIZE);
            int modY = (int) (position.y % Settings.CHUNK_SIZE);
            int modZ = (int) (position.z % Settings.CHUNK_SIZE);

            int blockX = (int) (Mathf.Floor(position.x) % Settings.CHUNK_SIZE) - (position.x < 0 ? 1 : 0);
            int blockY = (int) (Mathf.Floor(position.y) % Settings.CHUNK_SIZE) - (position.y < 0 ? 1 : 0);
            int blockZ = (int) (Mathf.Floor(position.z) % Settings.CHUNK_SIZE) - (position.z < 0 ? 1 : 0);

            int chunkX = (int) Mathf.Floor((int) position.x - modX);
            int chunkY = (int) Mathf.Floor((int) position.y - modY);
            int chunkZ = (int) Mathf.Floor((int) position.z - modZ);

            if (blockX < 0)
            {
                blockX += Settings.CHUNK_SIZE + 1;
                chunkX -= Settings.CHUNK_SIZE;
            }
            
            if (blockY < 0)
            {
                blockY += Settings.CHUNK_SIZE + 1;
                chunkY -= Settings.CHUNK_SIZE;
            }
            
            if (blockZ < 0)
            {
                blockZ += Settings.CHUNK_SIZE + 1;
                chunkZ -= Settings.CHUNK_SIZE;
            }
            
            
            Vector3 chunkPosition = new Vector3(chunkX, chunkY, chunkZ);

            string chunkName = BuildChunkName(chunkPosition);
            Utils.Debug(blockX, blockY, blockZ);
            return chunks.TryGetValue(chunkName, out Chunk chunk) ? chunk.chunkData[blockX, blockY, blockZ] : null;
        }

        public static Block GetWorldBlock(int x, int y, int z)
        {
            return GetWorldBlock(new Vector3(x, y, z));
        }

        private IEnumerator SaveChangedChunks()
        {
            Debug.Log("World Saving In Progress...");
            foreach (KeyValuePair<string, Chunk> chunkPair in chunks)
            {
                if (chunkPair.Value.changed) chunkPair.Value.Save();
            }

            Debug.Log("World Saved.");
            yield return null;
        }

        private void OnApplicationQuit()
        {
            StartCoroutine(SaveChangedChunks());
        }

        public static IEnumerator Flow(Block block, BlockType blockType, int strength, int maxSize)
        {
            if (maxSize <= 0) yield break;
            if (block == null) yield break;
            if (strength == 0) yield break;
            if (block.blockSetup.blockType != BlockType.Air) yield break;

            block.SetParent(block.owner.fluid);
            block.SetType(blockType);
            block.blockSetup.health = strength;
            block.owner.Redraw();
            yield return new WaitForSeconds(1);

            int x = (int) block.position.x;
            int y = (int) block.position.y;
            int z = (int) block.position.z;

            Block below = block.GetBlock(x, y - 1, z);
            if (below != null && (below.blockSetup.blockOpacity == BlockOpacity.Transparent || below.blockSetup.blockOpacity == BlockOpacity.Liquid))
            {
                instance.StartCoroutine(Flow(block.GetBlock(x, y - 1, z), blockType, strength, --maxSize));
                yield break;
            }
            else
            {
                --strength;
                --maxSize;

                coroutineQueue.Run(Flow(block.GetBlock(x - 1, y, z), blockType ,strength, maxSize));
                yield return new WaitForSeconds(1);
                
                coroutineQueue.Run(Flow(block.GetBlock(x + 1, y, z), blockType ,strength, maxSize));
                yield return new WaitForSeconds(1);
                
                coroutineQueue.Run(Flow(block.GetBlock(x, y, z - 1), blockType ,strength, maxSize));
                yield return new WaitForSeconds(1);
                
                coroutineQueue.Run(Flow(block.GetBlock(x, y, z + 1), blockType ,strength, maxSize));
                yield return new WaitForSeconds(1);
            }
        }

        public static IEnumerator DelayedFall(Block block, BlockType blockType)
        {
            yield return new WaitForSeconds(.25f);
            yield return Fall(block, blockType);
        }

        public static IEnumerator Fall(Block block, BlockType blockType)
        {
            Block thisBlock = block;
            Block previousBlock = null;

            while (true)
            {
                thisBlock.SetParent(thisBlock.owner.chunk);
                BlockType previousType = thisBlock.blockSetup.blockType;
                thisBlock.SetType(blockType);
                if (previousBlock == null) thisBlock.owner.Redraw();

                /*Block aboveBlock = thisBlock.GetBlock((int) thisBlock.position.x, (int) thisBlock.position.y + 1,
                    (int) thisBlock.position.z);
                if (aboveBlock.blockSetup.isFalling &&
                    (int) thisBlock.position.y == Settings.CHUNK_SIZE - 1)
                {
                    Debug.Log("Fall?");
                    yield return DelayedFall(aboveBlock, aboveBlock.blockSetup.blockType);
                }*/

                previousBlock?.SetType(previousType);
                previousBlock?.owner.Redraw();
                
                previousBlock = thisBlock;

                Vector3 position = thisBlock.position;
                thisBlock = thisBlock.GetBlock((int) position.x, (int) position.y - 1, (int) position.z);
                if (thisBlock == null) Utils.Debug((int) position.x, (int) position.y - 1, (int) position.z);
                Debug.Log(thisBlock + " " + thisBlock?.owner);
                thisBlock.owner.Redraw();
                yield return new WaitForSeconds(.1f);
                                

                if (thisBlock.blockSetup.blockOpacity == BlockOpacity.Solid)
                {
                    thisBlock.owner.Redraw();
                    if (thisBlock.owner != previousBlock.owner) previousBlock.owner.Redraw();
                    yield break;
                }
            }
        }
    }
}