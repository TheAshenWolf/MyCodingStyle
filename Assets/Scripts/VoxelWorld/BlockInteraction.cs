using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld
{
    public class BlockInteraction : MonoBehaviour
    {
        private const float BREAK_DELAY = .25f;
        
        [SerializeField] private GameObject cam;
        private float _time = 0;
        private Chunk _lastHitChunk;
        private Vector3 _lastHitPosition;
        private bool _hasLastBlock;

        private void Update()
        {
            _time += Time.deltaTime;


            if (Input.GetMouseButton(0) && _time > BREAK_DELAY)
            {
                _time = 0;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10))
                {
                    Vector3 hitBlock = hit.point - hit.normal / 2f;

                    Vector3 hitPosition = hit.collider.gameObject.transform.position;

                    int x = (int) Mathf.Round(hitBlock.x - hitPosition.x);
                    int y = (int) Mathf.Round(hitBlock.y - hitPosition.y);
                    int z = (int) Mathf.Round(hitBlock.z - hitPosition.z);

                    if (World.chunks.TryGetValue(hit.collider.gameObject.name, out Chunk hitChunk))
                    {
                        if (_hasLastBlock && _lastHitPosition != new Vector3(x, y, z))
                        {
                            _lastHitChunk.chunkData[(int) _lastHitPosition.x, (int) _lastHitPosition.y, (int) _lastHitPosition.z]
                                ?.Reset();
                        }
                        
                        _lastHitChunk = hitChunk;
                        _lastHitPosition = new Vector3(x, y, z);
                        _hasLastBlock = true;
                        
                        if (hitChunk.chunkData[x, y, z].HitBlock())
                        {
                            _hasLastBlock = false;
                            _time += BREAK_DELAY;
                            List<string> updates = new List<string>();

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
                                        chunk.Redraw();
                                    }
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