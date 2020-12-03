using System;
using TheAshenWolf;
using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelWorld
{
    public class Block
    {
        public Material material;
        public BlockType blockType;
        public GameObject parent;
        public Vector3 position;
        public Chunk owner;
        public Color[] colors = new Color[0];

        public bool isSolid;

        public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk chunk)
        {
            this.blockType = blockType;
            this.parent = parent;
            this.position = position;
            this.owner = chunk;
            isSolid = blockType != BlockType.Air;
        }

        private void CreateQuad(CubeSide side)
        {
            Vector3[] normals;
            Vector3[] vertices;

            Mesh mesh = new Mesh
            {
                name = "DynamicQuadMesh"
            };
            
            colors = new[]
            {
                Color.white, 
                Color.white, 
                Color.white, 
                Color.white
            };

            Vector2[] uvs;
            switch (blockType)
            {
                case BlockType.Grass:
                    switch (side)
                    {
                        case CubeSide.Top:
                            uvs = BlockUVs.GrassTop;
                            colors = new[]
                            {
                                new Color(.7f, 1f, .2f, .1f), 
                                new Color(.7f, 1f, .2f, .1f), 
                                new Color(.7f, 1f, .2f, .1f), 
                                new Color(.7f, 1f, .2f, .1f), 
                            };
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
                case BlockType.Bedrock:
                    uvs = BlockUVs.Bedrock;
                    break;
                case BlockType.CoalOre:
                    uvs = BlockUVs.CoalOre;
                    break;
                case BlockType.IronOre:
                    uvs = BlockUVs.IronOre;
                    break;
                case BlockType.GoldOre:
                    uvs = BlockUVs.GoldOre;
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

            if (colors.Length == mesh.vertices.Length)
            {
                mesh.colors = colors;
            }
            
            mesh.RecalculateBounds();

            GameObject quad = new GameObject("Quad");
            quad.transform.position = position;
            quad.transform.parent = parent.transform;

            MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        private int ConvertBlockIndexToLocal(int index)
        {
            if (index == -1) index = World.chunkSize - 1;
            else if (index == World.chunkSize) index = 0;

            return index;
        }

        private bool HasSolidNeighbour(int x, int y, int z)
        {
            Block[,,] chunkData;

            if (x < 0 || x >= World.chunkSize ||
                y < 0 || y >= World.chunkSize ||
                z < 0 || z >= World.chunkSize)
            {
                Vector3 neighbourChunkPos = parent.transform.position +
                                            new Vector3((x - (int) position.x) * World.chunkSize,
                                                (y - (int) position.y) * World.chunkSize,
                                                (z - (int) position.z) * World.chunkSize);

                string neighbourName = World.BuildChunkName(neighbourChunkPos);

                x = ConvertBlockIndexToLocal(x);
                y = ConvertBlockIndexToLocal(y);
                z = ConvertBlockIndexToLocal(z);

                if (World.chunks.TryGetValue(neighbourName, out Chunk neighbourChunk))
                {
                    chunkData = neighbourChunk.chunkData;
                }
                else return false;
            }
            else chunkData = owner.chunkData;

            try
            {
                return chunkData[x, y, z].isSolid;
            }
            catch
            {
                return false;
            }
        }

        public void Draw()
        {
            if (blockType == BlockType.Air) return;

            if (!HasSolidNeighbour((int) position.x, (int) position.y, (int) position.z - 1)) 
                CreateQuad(CubeSide.Back);
            
            if (!HasSolidNeighbour((int) position.x, (int) position.y, (int) position.z + 1))
                CreateQuad(CubeSide.Front);
            
            if (!HasSolidNeighbour((int) position.x, (int) position.y - 1, (int) position.z))
                CreateQuad(CubeSide.Bottom);
            
            if (!HasSolidNeighbour((int) position.x, (int) position.y + 1, (int) position.z)) 
                CreateQuad(CubeSide.Top);
            
            if (!HasSolidNeighbour((int) position.x - 1, (int) position.y, (int) position.z)) 
                CreateQuad(CubeSide.Left);
            
            if (!HasSolidNeighbour((int) position.x + 1, (int) position.y, (int) position.z))
                CreateQuad(CubeSide.Right);
        }
    }
}