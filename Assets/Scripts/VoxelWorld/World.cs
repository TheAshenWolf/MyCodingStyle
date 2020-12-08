using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VoxelWorld
{
    public class World : MonoBehaviour
    {
        public GameObject player;
        public Material textureAtlas;
        public static int chunkSize = 16;
        public static int columnHeight = 16;
        public static int worldSize = 4;
        public static int radius = 1;
        public static Dictionary<string, Chunk> chunks;

        [SerializeField] private Slider slider;
        [SerializeField] private Camera loadingCamera;
        [SerializeField] private TextMeshProUGUI statusText;


        private void Start()
        {
            player.SetActive(false);
            chunks = new Dictionary<string, Chunk>();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        public void StartLoading()
        {
            slider.gameObject.SetActive(true);
            statusText.gameObject.SetActive(true);
            StartCoroutine(BuildWorld());
        }

        public static string BuildChunkName(Vector3 position)
        {
            return (int) position.x + "_" + (int) position.y + "_" + (int) position.z;
        }

        public IEnumerator BuildChunkColumn()
        {
            for (int i = 0; i < columnHeight; i++)
            {
                Vector3 chunkPosition = new Vector3(transform.position.x, i * chunkSize, transform.position.z);
                
                Chunk chunk = new Chunk(chunkPosition, textureAtlas);
                chunk.chunk.transform.parent = transform;
                chunks.Add(chunk.chunk.name, chunk);
            }

            foreach (KeyValuePair<string, Chunk> chunk in chunks)
            {
                chunk.Value.DrawChunk();
                yield return null;
            }
        }

        public IEnumerator BuildWorld()
        {
            statusText.text = "Building World";
            
            int playerPositionX = (int) Mathf.Floor(player.transform.position.x / chunkSize);
            int playerPositionZ = (int) Mathf.Floor(player.transform.position.z / chunkSize);

            float totalChunks = (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2;
            int processCount = 0;
            
            for (int z = -radius; z <= radius; z++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    for (int y = 0; y < columnHeight; y++)
                    {
                        Vector3 chunkPosition = new Vector3((x + playerPositionX) * chunkSize, y * chunkSize, (z + playerPositionZ)  * chunkSize);
                        Chunk chunk = new Chunk(chunkPosition, textureAtlas);
                        chunk.chunk.transform.parent = this.transform;
                        chunks.Add(chunk.chunk.name, chunk);
                        processCount++;
                        slider.value = processCount / totalChunks * 100;
                        yield return null;
                    }
                }
            }

            statusText.text = "Rendering";
            
            foreach (KeyValuePair<string, Chunk> chunk in chunks)
            {
                chunk.Value.DrawChunk();
                processCount++;
                slider.value = processCount / totalChunks * 100;
                yield return null;
            }
            
            slider.gameObject.SetActive(false);
            statusText.gameObject.SetActive(false);
            loadingCamera.gameObject.SetActive(false);
            player.SetActive(true);
        }
    }
}