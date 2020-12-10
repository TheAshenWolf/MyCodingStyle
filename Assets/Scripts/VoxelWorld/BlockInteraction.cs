using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld
{
    public class BlockInteraction : MonoBehaviour
    {
        [SerializeField] private GameObject cam;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10))
                {
                    Vector3 hitBlock = hit.point - hit.normal / 2f;

                    Vector3 hitPosition = hit.collider.gameObject.transform.position;

                    int x = (int) Mathf.Round(hitBlock.x - hitPosition.x);
                    int y = (int) Mathf.Round(hitBlock.y - hitPosition.y);
                    int z = (int) Mathf.Round(hitBlock.z - hitPosition.z);

                    List<string> updates = new List<string>();

                    updates.Add(hit.collider.gameObject.name);


                    if (x == 0)
                    {
                        updates.Add(World.BuildChunkName(hitPosition - new Vector3(World.chunkSize, 0, 0)));
                    }

                    if (y == 0)
                    {
                        updates.Add(World.BuildChunkName(hitPosition - new Vector3(0, World.chunkSize, 0)));
                    }

                    if (z == 0)
                    {
                        updates.Add(World.BuildChunkName(hitPosition - new Vector3(0, 0, World.chunkSize)));
                    }

                    if (x == World.chunkSize - 1)
                    {
                        updates.Add(World.BuildChunkName(hitPosition + new Vector3(World.chunkSize, 0, 0)));
                    }

                    if (y == World.chunkSize - 1)
                    {
                        updates.Add(World.BuildChunkName(hitPosition + new Vector3(0, World.chunkSize, 0)));
                    }

                    if (z == World.chunkSize - 1)
                    {
                        updates.Add(World.BuildChunkName(hitPosition + new Vector3(0, 0, World.chunkSize)));
                    }


                    foreach (string chunkName in updates)
                    {
                        if (World.chunks.TryGetValue(chunkName, out Chunk chunk))
                        {
                            // todo: recalculate the components instead
                            if (chunk.chunkData[x, y, z].blockType != BlockType.Bedrock)
                            {
                                DestroyImmediate(chunk.chunk.GetComponent<MeshFilter>());
                                DestroyImmediate(chunk.chunk.GetComponent<MeshRenderer>());
                                DestroyImmediate(chunk.chunk.GetComponent<Collider>());
                                chunk.chunkData[x, y, z].SetType(BlockType.Air);
                                chunk.DrawChunk();
                            }
                        }
                    }
                }
            }
        }
    }
}