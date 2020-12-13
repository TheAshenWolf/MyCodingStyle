using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld
{
    public class BlockInteraction : MonoBehaviour
    {
        private const float INTERACTION_DELAY = .25f;

        [SerializeField] private GameObject cam;
        private float _time = 0;
        private Chunk _lastHitChunk;
        private Vector3 _lastHitPosition;
        private bool _hasLastBlock;

        private void Update()
        {
            _time += Time.deltaTime;


            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && _time > INTERACTION_DELAY)
            {
                _time = 0;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10))
                {
                    
                    Debug.DrawRay(hit.point, hit.normal / 2, Color.magenta, 10f);
                    
                    if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out Chunk hitChunk)) return;

                    Vector3 hitBlockPosition;

                    if (Input.GetMouseButton(0))
                        hitBlockPosition = hit.point - hit.normal / 2f;
                    else hitBlockPosition = hit.point + hit.normal / 2f;

                    Block block = World.GetWorldBlock(hitBlockPosition ,hitChunk);
                    hitChunk = block.owner;

                    Vector3 hitChunkPosition = hit.collider.gameObject.transform.position;

                    // Vector3 hitColliderPosition = hit.collider.transform.position;
                    int x = (int) block.position.x; // (Mathf.Round(hitBlockPosition.x) - hitColliderPosition.x);
                    int y = (int) block.position.y; // (Mathf.Round(hitBlockPosition.y) - hitColliderPosition.y);
                    int z = (int) block.position.z; // (Mathf.Round(hitBlockPosition.z) - hitColliderPosition.z);

                    bool updateNeighbours;

                    if (Input.GetMouseButton(0))
                    {
                        updateNeighbours = hitChunk.chunkData[x, y, z].HitBlock();
                        if (_hasLastBlock && _lastHitPosition != new Vector3(x, y, z))
                        {
                            _lastHitChunk.chunkData[(int) _lastHitPosition.x, (int) _lastHitPosition.y,
                                    (int) _lastHitPosition.z]
                                ?.Reset();
                        }
                        
                        if (updateNeighbours) _time += INTERACTION_DELAY;
                        
                        _lastHitChunk = hitChunk;
                        _lastHitPosition = new Vector3(x, y, z);
                        _hasLastBlock = true;
                    }
                    else
                    {
                        updateNeighbours = block.BuildBlock(BlockType.Stone);
                    }

                    if (updateNeighbours)
                    {
                        _hasLastBlock = false;
                        List<string> updates = new List<string>();

                        if ((int) block.position.x == 0)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition - new Vector3(World.chunkSize, 0, 0)));
                        }

                        if ((int) block.position.y == 0)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition - new Vector3(0, World.chunkSize, 0)));
                        }

                        if ((int) block.position.z == 0)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition - new Vector3(0, 0, World.chunkSize)));
                        }

                        if ((int) block.position.x == World.chunkSize - 1)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition + new Vector3(World.chunkSize, 0, 0)));
                        }

                        if ((int) block.position.y == World.chunkSize - 1)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition + new Vector3(0, World.chunkSize, 0)));
                        }

                        if ((int) block.position.z == World.chunkSize - 1)
                        {
                            updates.Add(World.BuildChunkName(hitChunkPosition + new Vector3(0, 0, World.chunkSize)));
                        }

                        foreach (string chunkName in updates)
                        {
                            if (World.chunks.TryGetValue(chunkName, out Chunk chunk))
                            {
                                // todo: recalculate the components instead
                                if (chunk.chunkData[x, y, z].blockType != BlockType.Bedrock)
                                {
                                    chunk.Redraw();
                                }
                            }
                        }
                    }
                }
                else if (_hasLastBlock)
                {
                    _lastHitChunk.chunkData[(int) _lastHitPosition.x, (int) _lastHitPosition.y,
                        (int) _lastHitPosition.z]?.Reset();
                    _hasLastBlock = false;
                }
            }

            else if (Input.GetMouseButtonUp(0) && _hasLastBlock)
            {
                _lastHitChunk.chunkData[(int) _lastHitPosition.x, (int) _lastHitPosition.y, (int) _lastHitPosition.z]
                    ?.Reset();
                _hasLastBlock = false;
            }
        }
    }
}