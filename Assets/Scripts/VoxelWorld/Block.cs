using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace VoxelWorld
{
    public class Block
    {
        public readonly Chunk owner;


        private readonly Material _material;

        private GameObject _parent;
        public Vector3 position;

        private Color[] _colors = new Color[0];

        private int _actualHealth;
        private Crack crackTexture = Crack.Crack0;

        public BlockSetup blockSetup;

        public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk chunk)
        {
            this.blockSetup = GenerateBlockSetup(blockType);
            this._parent = parent;
            this.position = position;
            this.owner = chunk;
            _actualHealth = blockSetup.health * blockSetup.toughness;
        }

        public Block(BlockType blockType, Vector3 position, GameObject parent, Material material)
        {
            this.blockSetup = GenerateBlockSetup(blockType);
            this.position = position;
            this._parent = parent;
            this._material = material;
            Draw(true);
        }

        public bool HitBlock()
        {
            _actualHealth--;
            crackTexture += (int) (10 / blockSetup.health);

            if (_actualHealth <= 0)
            {
                blockSetup = GenerateBlockSetup(BlockType.Air);
                crackTexture = Crack.Crack0;
                owner.Redraw();
                Block blockAbove = GetBlock((int) position.x, (int) position.y + 1, (int) position.z);
                if (blockAbove.blockSetup.isFalling)
                {
                    owner.world.StartCoroutine(World.Fall(blockAbove, blockAbove.blockSetup.blockType));
                }
                return true;
            }

            owner.Redraw();
            return false;
        }

        public void SetType(BlockType blockType)
        {
            this.blockSetup = GenerateBlockSetup(blockType);
            crackTexture = Crack.Crack0;
            _actualHealth = blockSetup.health * blockSetup.toughness;
        }

        private void CreateQuad(CubeSide side, bool handBlock = false)
        {
            Vector3[] normals;
            Vector3[] vertices;

            List<Vector2> secondaryUVs = new List<Vector2>();

            Mesh mesh = new Mesh
            {
                name = "DynamicQuadMesh"
            };

            _colors = new[]
            {
                Color.white,
                Color.white,
                Color.white,
                Color.white
            };

            Vector2[] uvs;
            switch (blockSetup.blockType)
            {
                case BlockType.Grass:
                    switch (side)
                    {
                        case CubeSide.Top:
                            uvs = BlockUVs.GrassTop;
                            _colors = new[]
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
                case BlockType.Cobblestone:
                    uvs = BlockUVs.Cobblestone;
                    break;
                case BlockType.BookShelf:
                    uvs = (side == CubeSide.Top || side == CubeSide.Bottom)
                        ? BlockUVs.Planks
                        : BlockUVs.BookshelfSide;
                    break;
                case BlockType.Sand:
                    uvs = BlockUVs.Sand;
                    break;
                case BlockType.Gravel:
                    uvs = BlockUVs.Gravel;
                    break;
                case BlockType.Water:
                    uvs = BlockUVs.Water;
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

            if (_colors.Length == mesh.vertices.Length)
            {
                mesh.colors = _colors;
            }

            mesh.RecalculateBounds();

            GameObject quad = new GameObject("Quad");
            if (!handBlock) quad.transform.position = position;
            quad.transform.parent = _parent.transform;
            if (handBlock)
            {
                quad.transform.localPosition = position;
                quad.transform.localScale = Vector3.one;
                quad.transform.localRotation = Quaternion.identity;
            }

            MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            if (handBlock)
            {
                MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();
                meshRenderer.material = _material;
            }
        }

        private int ConvertBlockIndexToLocal(int index)
        {
            int mod = index % Settings.CHUNK_SIZE;
            mod = mod >= 0 ? mod : Settings.CHUNK_SIZE + mod;
            return mod;
        }

        public Block GetBlock(int x, int y, int z)
        {
            Block[,,] chunkData;

            if (x < 0 || x >= Settings.CHUNK_SIZE ||
                y < 0 || y >= Settings.CHUNK_SIZE ||
                z < 0 || z >= Settings.CHUNK_SIZE)
            {
                Vector3 neighbourChunkPos = _parent.transform.position +
                                            new Vector3(
                                                (x - (int) position.x) * Settings.CHUNK_SIZE,
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
                else
                {
                    return null;
                }
            }
            else chunkData = owner.chunkData;
            return chunkData[x, y, z];
        }

        private bool HasSolidNeighbour(int x, int y, int z)
        {
            try
            {
                return (GetBlock(x, y, z).blockSetup.blockOpacity == BlockOpacity.Solid ||
                        GetBlock(x, y, z).blockSetup.blockType == blockSetup.blockType);
            }
            catch
            {
                return false;
            }
        }

        public void Draw(bool handBlock = false)
        {
            if (blockSetup.blockType == BlockType.Air) return;

            if (handBlock || !HasSolidNeighbour((int) position.x, (int) position.y, (int) position.z - 1))
                CreateQuad(CubeSide.Back, handBlock);

            if (handBlock || !HasSolidNeighbour((int) position.x, (int) position.y, (int) position.z + 1))
                CreateQuad(CubeSide.Front, handBlock);

            if (handBlock || !HasSolidNeighbour((int) position.x, (int) position.y - 1, (int) position.z))
                CreateQuad(CubeSide.Bottom, handBlock);

            if (handBlock || !HasSolidNeighbour((int) position.x, (int) position.y + 1, (int) position.z))
                CreateQuad(CubeSide.Top, handBlock);

            if (handBlock || !HasSolidNeighbour((int) position.x - 1, (int) position.y, (int) position.z))
                CreateQuad(CubeSide.Left, handBlock);

            if (handBlock || !HasSolidNeighbour((int) position.x + 1, (int) position.y, (int) position.z))
                CreateQuad(CubeSide.Right, handBlock);
        }

        public void Reset()
        {
            crackTexture = Crack.Crack0;
            _actualHealth = blockSetup.health;
            owner.Redraw();
        }

        public bool BuildBlock(BlockType blockType, GameObject parent)
        {
            SetParent(parent);
            if (blockType == BlockType.Water || blockType == BlockType.Lava)
            {
                owner.world.StartCoroutine(World.Flow(this, blockType, blockSetup.health, 8));
            }
            else if (blockType == BlockType.Sand || blockType == BlockType.Gravel)
            {
                owner.world.StartCoroutine(World.Fall(this, blockType));
            }
            else
            {
                SetType(blockType);
                owner.Redraw();
            }

            return true;
        }

        private BlockSetup GenerateBlockSetup(BlockType blockType)
        {
            BlockSetup setup = new BlockSetup()
            {
                blockType = blockType,
                health = blockType == BlockType.Water ? 8 : 5,
                toughness = 1,
                blockOpacity = GetBlockOpacity(blockType),
                isFalling = (blockType == BlockType.Gravel || blockType == BlockType.Sand)
            };

            return setup;
        }

        public static BlockOpacity GetBlockOpacity(BlockType blockType)
        {
            if (blockType == BlockType.Air) return BlockOpacity.Transparent;
            if (blockType == BlockType.Water) return BlockOpacity.Liquid;
            return BlockOpacity.Solid;
        }

        public void SetParent(GameObject parent)
        {
            _parent = parent;
        }
    }
}