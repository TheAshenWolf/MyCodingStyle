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
                        chunkData[x,y,z] = new Block(Random.Range(0 , 100) > 50  ? BlockType.Stone : BlockType.Air, position, chunk, this);
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