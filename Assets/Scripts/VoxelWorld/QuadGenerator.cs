using System;
using TheAshenWolf;
using UnityEngine;

namespace VoxelWorld
{
    public class QuadGenerator : MonoBehaviour
    {
        public Material cubeMaterial;

        public enum CubeSide
        {
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back
        }

        private void Start()
        {
            GenerateCube();
        }

        private void CreateQuad(CubeSide side)
        {
            Vector3[] normals;
            Vector3[] vertices;

            Mesh mesh = new Mesh
            {
                name = "DynamicQuadMesh"
            };

            // UVs - Thinking in binary, the 0s and 1s are going up from 0 to 3
            Vector2 uv00 = new Vector2(0, 0);
            Vector2 uv01 = new Vector2(0, 1);
            Vector2 uv10 = new Vector2(1, 0);
            Vector2 uv11 = new Vector2(1, 1);

            // Vertices - If we consider -0.5f a 0 and 0.5f a 1, we have the same behaviour as above
            Vector3 v0 = new Vector3(-0.5f, -0.5f, -0.5f); // p3
            Vector3 v1 = new Vector3(-0.5f, -0.5f, 0.5f); // p0
            Vector3 v2 = new Vector3(-0.5f, 0.5f, -0.5f); // p7
            Vector3 v3 = new Vector3(-0.5f, 0.5f, 0.5f); // p4
            Vector3 v4 = new Vector3(0.5f, -0.5f, -0.5f); // p2
            Vector3 v5 = new Vector3(0.5f, -0.5f, 0.5f); // p1
            Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f); // p6
            Vector3 v7 = new Vector3(0.5f, 0.5f, 0.5f); // p5

            switch (side)
            {
                case CubeSide.Top:
                    vertices = new Vector3[]
                    {
                        v2, v6, v7, v3
                    };
                    normals = new Vector3[]
                    {
                        Vector3.up,
                        Vector3.up,
                        Vector3.up,
                        Vector3.up,
                    };
                    break;
                case CubeSide.Bottom:
                    vertices = new Vector3[]
                    {
                        v1, v5, v4, v0
                    };
                    normals = new Vector3[]
                    {
                        Vector3.down,
                        Vector3.down,
                        Vector3.down,
                        Vector3.down,
                    };
                    break;
                case CubeSide.Left:
                    vertices = new Vector3[]
                    {
                        v2, v3, v1, v0
                    };
                    normals = new Vector3[]
                    {
                        Vector3.left,
                        Vector3.left,
                        Vector3.left,
                        Vector3.left,
                    };
                    break;
                case CubeSide.Right:
                    vertices = new Vector3[]
                    {
                        v7, v6, v4, v5
                    };
                    normals = new Vector3[]
                    {
                        Vector3.right,
                        Vector3.right,
                        Vector3.right,
                        Vector3.right,
                    };
                    break;
                case CubeSide.Front:
                    vertices = new Vector3[]
                    {
                        v3, v7, v5, v1
                    };
                    normals = new Vector3[]
                    {
                        Vector3.forward,
                        Vector3.forward,
                        Vector3.forward,
                        Vector3.forward,
                    };
                    break;
                case CubeSide.Back:
                    vertices = new Vector3[]
                    {
                        v6, v2, v0, v4
                    };
                    normals = new Vector3[]
                    {
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            Vector2[] uvs = {uv11, uv01, uv00, uv10};
            int[] triangles = {3, 1, 0, 3, 2, 1};

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            GameObject quad = new GameObject("Quad");
            quad.transform.parent = gameObject.transform;
            MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer quadRenderer = quad.AddComponent<MeshRenderer>();
            quadRenderer.material = cubeMaterial;
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
            meshRenderer.material = cubeMaterial;
            
            // Delete all children (the quads)
            transform.DestroyAllChildren();
        }

        private void GenerateCube()
        {
            CreateQuad(CubeSide.Back);
            CreateQuad(CubeSide.Front);
            CreateQuad(CubeSide.Bottom);
            CreateQuad(CubeSide.Top);
            CreateQuad(CubeSide.Left);
            CreateQuad(CubeSide.Right);
            
            CombineQuads();
        }
    }
}