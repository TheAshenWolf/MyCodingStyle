using System;
using System.Collections;
using System.Collections.Generic;
using TheAshenWolf;
using UnityEngine;

namespace VoxelWorld
{
    public class Chunk : MonoBehaviour
    {
        public Material material;

        private void Start()
        {
            StartCoroutine(BuildChunk(4, 4, 4));
        }


        IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        Block block = new Block(BlockType.Dirt, position, gameObject, material);
                        block.Draw();
                        yield return null;
                    }
                }
            }

            CombineQuads();
        }
        
        private void CombineQuads()
        {
            // Combining
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            
            // Creating new mesh
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
            
            // Assign mesh
            meshFilter.mesh.CombineMeshes(combine);
            
            // Add renderer
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            
            // Delete all children (the quads)
            transform.DestroyAllChildren();
        }
    }
}