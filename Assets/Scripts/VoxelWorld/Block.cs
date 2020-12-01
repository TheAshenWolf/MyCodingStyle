using System;
using TheAshenWolf;
using UnityEngine;

namespace VoxelWorld
{
    public class Block
    {
        public Material material;
        public BlockType blockType;
        public GameObject parent;
        public Vector3 position;

        public Block(BlockType blockType, Vector3 position, GameObject parent, Material material)
        {
            this.blockType = blockType;
            this.parent = parent;
            this.position = position;
            this.material = material;
        }

        private void CreateQuad(CubeSide side)
        {
            Vector3[] normals;
            Vector3[] vertices;

            Mesh mesh = new Mesh
            {
                name = "DynamicQuadMesh"
            };
            
            Vector2[] uvs;
            switch (blockType)
            {
                case BlockType.Grass:
                    switch (side)
                    {
                        case CubeSide.Top:
                            uvs = BlockUVs.GrassTop;
                            break;
                        case CubeSide.Bottom:
                            uvs = BlockUVs.Dirt;
                            break;
                        default:
                            uvs = BlockUVs.GrassSide;
                            break;
                    }
                    break;
                case BlockType.Dirt:
                    uvs = BlockUVs.Dirt;
                    break;
                case BlockType.Stone:
                    uvs = BlockUVs.Stone;
                    break;
                case BlockType.Planks:
                    uvs = BlockUVs.Planks;
                    break;
                case BlockType.Brick:
                    uvs = BlockUVs.Brick;
                    break;
                case BlockType.Wood:
                    uvs = (side == CubeSide.Top || side == CubeSide.Bottom)
                        ? BlockUVs.WoodVertical
                        : BlockUVs.WoodHorizontal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
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
            int[] triangles = {3, 1, 0, 3, 2, 1};

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            GameObject quad = new GameObject("Quad");
            quad.transform.position = position;
            quad.transform.parent = parent.transform;
            
            MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            MeshRenderer quadRenderer = quad.AddComponent<MeshRenderer>();
            quadRenderer.material = material;
        }
        
        public void Draw()
        {
            CreateQuad(CubeSide.Back);
            CreateQuad(CubeSide.Front);
            CreateQuad(CubeSide.Bottom);
            CreateQuad(CubeSide.Top);
            CreateQuad(CubeSide.Left);
            CreateQuad(CubeSide.Right);
        }
    }
}