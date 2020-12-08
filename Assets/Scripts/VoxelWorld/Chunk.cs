using System;
using System.Collections;
using System.Collections.Generic;
using TheAshenWolf;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VoxelWorld
{
    public class Chunk
    {
        public Material material;
        public Block[,,] chunkData;
        public GameObject chunk;


        public Chunk(Vector3 position, Material material)
        {
            chunk = new GameObject(World.BuildChunkName(position));
            chunk.transform.position = position;
            this.material = material;
            BuildChunk();
        }
        
        
        public void BuildChunk()
        {
            chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
            
            for (int z = 0; z < World.chunkSize; z++)
            {
                for (int y = 0; y < World.chunkSize; y++)
                {
                    for (int x = 0; x < World.chunkSize; x++)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        int worldX = (int)(x + chunk.transform.position.x);
                        int worldY = (int)(y + chunk.transform.position.y);
                        int worldZ = (int)(z + chunk.transform.position.z);
                        
                        BlockType blockType;

                        // Default "chunk" of materials grass - dirt - stone - bedrock
                        if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ,  pers: 0.04f) < .4f) blockType = BlockType.Air;
                        else if (worldY == Utils.GenerateHeight(worldX, worldZ)) blockType = BlockType.Grass;
                        else if (worldY < Utils.GenerateHeight(worldX, worldZ) && worldY > Utils.GenerateStoneHeight(worldX, worldZ)) blockType = BlockType.Dirt;
                        else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ) && worldY > 1)
                        {
                            if (worldY < Utils.maxHeight / 4 * 3 &&
                                Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.3f, 2, .15f) < .376f)
                            {
                                Debug.Log("Coal!");
                                blockType = BlockType.CoalOre;
                            }
                            else if (worldY < Utils.maxHeight / 8 * 2.5f &&
                                Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.25f, 3, .29f) < .37f)
                            {
                                Debug.Log("Iron!");
                                blockType = BlockType.IronOre;
                            }
                            else if (worldY < Utils.maxHeight / 16 * 1.75f &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.2f, 2, .22f) < .35f)
                            {
                                Debug.Log("Gold!");
                                blockType = BlockType.GoldOre;
                            }
                            else if (worldY < 20 &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.19f, 3, .16f) < .35f)
                            {
                                Debug.Log("Redstone!");
                                blockType = BlockType.RedstoneOre;
                            }
                            
                                
                            else blockType = BlockType.Stone;
                        }
                        else blockType = BlockType.Air;
                        
                        if ((worldY <= 1) && (worldY > 0)) blockType = BlockType.Bedrock;
                        
                        
                        
                        chunkData[x,y,z] = new Block(blockType, position, chunk, this);
                    }
                }
            }
        }

        public void DrawChunk()
        {
            // Rendering
            for (int z = 0; z < World.chunkSize; z++)
            {
                for (int y = 0; y < World.chunkSize; y++)
                {
                    for (int x = 0; x < World.chunkSize; x++)
                    {
                        chunkData[x,y,z].Draw();
                    }
                }
            }

            CombineQuads();

            MeshCollider meshCollider = chunk.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
        }
        
        private void CombineQuads()
        {
            // Combining
            MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            
            // Creating new mesh
            MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
            
            // Assign mesh
            meshFilter.mesh.CombineMeshes(combine);
            
            // Add renderer
            MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            
            // Delete all children (the quads)
            chunk.transform.DestroyAllChildren();
        }
    }
}