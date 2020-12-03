using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace VoxelWorld
{
    public class World : MonoBehaviour
    {
        public Material textureAtlas;
        public static int chunkSize = 16;
        public static int columnHeight = 16;
        public static int worldSize = 4;
        public static Dictionary<string, Chunk> chunks;


        private void Start()
        {
            chunks = new Dictionary<string, Chunk>();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
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
            for (int z = 0; z < World.worldSize; z++)
            {
                for (int x = 0; x < World.worldSize; x++)
                {
                    for (int y = 0; y < World.columnHeight; y++)
                    {
                        Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                        Chunk chunk = new Chunk(chunkPosition, textureAtlas);
                        chunk.chunk.transform.parent = this.transform;
                        chunks.Add(chunk.chunk.name, chunk);
                    }
                }
            }
            
            foreach (KeyValuePair<string, Chunk> chunk in chunks)
            {
                chunk.Value.DrawChunk();
                yield return null;
            }
        }
    }
}