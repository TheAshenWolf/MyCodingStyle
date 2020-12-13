using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

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
        public int toughness = 1;
        public int health = 5;
        private int _actualHealth;
        public Crack crackTexture = Crack.Crack0;


        public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk chunk)
        {
            this.blockType = blockType;
            this.parent = parent;
            this.position = position;
            this.owner = chunk;
            isSolid = blockType != BlockType.Air;
            _actualHealth = health * toughness;
        }

        public bool HitBlock()
        {
            _actualHealth--;
            crackTexture += (int)(10 / health);

            if (_actualHealth <= 0)
            {
                blockType = BlockType.Air;
                isSolid = false;
                crackTexture = Crack.Crack0;
                owner.Redraw();
                return true;
            }

            owner.Redraw();
            return false;
        }

        public void SetType(BlockType blockType)
        {
            this.blockType = blockType;
            isSolid = blockType != BlockType.Air;
            crackTexture = Crack.Crack0;
            _actualHealth = health * toughness;
        }

        private void CreateQuad(CubeSide side)
        {
            Vector3[] normals;
            Vector3[] vertices;
            
            List<Vector2> secondaryUVs = new List<Vector2>();

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
                case BlockType.RedstoneOre:
                    uvs = BlockUVs.RedstoneOre;
                    break;


                default:
                    throw new ArgumentOutOfRangeException();
            }

            secondaryUVs = BlockUVs.GetCrack(crackTexture).ToList();

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
            mesh.SetUVs(1, secondaryUVs);
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
            if (index == -1) index = Settings.CHUNK_SIZE - 1;
            else if (index == Settings.CHUNK_SIZE) index = 0;

            return index;
        }

        private bool HasSolidNeighbour(int x, int y, int z)
        {
            Block[,,] chunkData;

            if (x < 0 || x >= Settings.CHUNK_SIZE ||
                y < 0 || y >= Settings.CHUNK_SIZE ||
                z < 0 || z >= Settings.CHUNK_SIZE)
            {
                Vector3 neighbourChunkPos = parent.transform.position +
                                            new Vector3((x - (int) position.x) * Settings.CHUNK_SIZE,
                                                (y - (int) position.y) * Settings.CHUNK_SIZE,
                                                (z - (int) position.z) * Settings.CHUNK_SIZE);

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

        public void Reset()
        {
            crackTexture = Crack.Crack0;
            _actualHealth = health;
            owner.Redraw();
        }

        public bool BuildBlock(BlockType blockType)
        {
            SetType(blockType);
            owner.Redraw();
            return true;
        }
    }
}