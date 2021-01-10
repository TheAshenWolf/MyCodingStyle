using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TheAshenWolf;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VoxelWorld
{
    public class Chunk
    {
        private readonly Material _solidMaterial;
        private readonly Material _fluidMaterial;
        public Block[,,] chunkData;
        public readonly GameObject chunk;
        public readonly GameObject fluid;
        public ChunkState chunkState;
        private BlockData _blockData;
        public bool changed;
        public World world;
        private bool _treesGenerated;

        private string BuildChunkFileName(Vector3 position)
        {
            return Application.persistentDataPath + "/saveData/Chunk_" + (int) position.x + "_" + (int) position.y +
                   "_" + (int) position.z + "_" + Settings.CHUNK_SIZE + "_" + Settings.RENDER_DISTANCE + ".dat";
        }

        private bool Load()
        {
            string chunkFile = BuildChunkFileName(chunk.transform.position);
            if (File.Exists(chunkFile))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(chunkFile, FileMode.Open);
                _blockData = new BlockData();
                _blockData = (BlockData) binaryFormatter.Deserialize(file);
                file.Close();

                return true;
            }

            return false;
        }

        public void Save()
        {
            string chunkFile = BuildChunkFileName(chunk.transform.position);
            if (!File.Exists(chunkFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(chunkFile) ?? throw new Exception("Path is null"));
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
            _blockData = new BlockData(chunkData);
            binaryFormatter.Serialize(file, _blockData);
            file.Close();

            changed = false;
        }

        public Chunk(Vector3 position, Material solidMaterial, Material fluidMaterial)
        {
            string chunkName = World.BuildChunkName(position);
            chunk = new GameObject(chunkName);
            chunk.transform.position = position + new Vector3(0.5f, 0.5f, 0.5f);

            fluid = new GameObject(World.BuildChunkName(position) + "_F");
            fluid.transform.position = position + new Vector3(0.5f, 0.5f, 0.5f);

            _solidMaterial = solidMaterial;
            _fluidMaterial = fluidMaterial;
            BuildChunk();
        }

        private void BuildChunk()
        {
            bool loadingFromFile;
            loadingFromFile = Load();

            chunkData = new Block[Settings.CHUNK_SIZE, Settings.CHUNK_SIZE, Settings.CHUNK_SIZE];

            for (int z = 0; z < Settings.CHUNK_SIZE; z++)
            {
                for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                {
                    for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                    {
                        bool addToFluid = false;
                        Vector3 position = new Vector3(x, y, z);
                        Vector3 chunkPosition = chunk.transform.position;
                        int worldX = (int) (x + chunkPosition.x);
                        int worldY = (int) (y + chunkPosition.y);
                        int worldZ = (int) (z + chunkPosition.z);

                        if (loadingFromFile)
                        {
                            chunkData[x, y, z] = new Block(_blockData.blockMatrix[x, y, z], position,
                                Block.GetBlockOpacity(_blockData.blockMatrix[x, y, z]) == BlockOpacity.Solid
                                    ? chunk
                                    : fluid,
                                this);
                            continue;
                        }

                        BlockType blockType = BlockType.Stone;

                        // Default "chunk" of materials grass - dirt - stone - bedrock

                        if (worldY == Utils.GenerateHeight(worldX, worldZ))
                        {
                            if (worldY <= Settings.OCEAN_HEIGHT +
                                Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, .4f, 2) * 3 - 1)
                            {
                                blockType = Settings.OCEAN_HEIGHT - worldY > 4 ? BlockType.Gravel : BlockType.Sand;
                            }
                            else if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, .4f, 2) < .3f)
                            {
                                blockType = BlockType.TreeSeed;
                            }
                            else
                            {
                                blockType = BlockType.Grass;
                            }
                        }
                        else if (worldY < Utils.GenerateHeight(worldX, worldZ) &&
                                 worldY > Utils.GenerateStoneHeight(worldX, worldZ)) blockType = BlockType.Dirt;
                        else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ) && worldY > 1)
                        {
                            if (worldY < Utils.MAX_HEIGHT / 4 * 3 &&
                                Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.3f, 2, .15f) < .376f)
                            {
                                blockType = BlockType.CoalOre;
                            }
                            else if (worldY < Utils.MAX_HEIGHT / 8f * 2.5f &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.25f, 3, .29f) < .37f)
                            {
                                blockType = BlockType.IronOre;
                            }
                            else if (worldY < Utils.MAX_HEIGHT / 16f * 1.75f &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.2f, 2, .22f) < .35f)
                            {
                                blockType = BlockType.GoldOre;
                            }
                            else if (worldY < 20 &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.19f, 3, .16f) < .35f)
                            {
                                blockType = BlockType.RedstoneOre;
                            }

                            if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, persistence: 0.04f) < .4f &&
                                (blockType != BlockType.Water))
                                blockType = BlockType.Air;


                            // else blockType = BlockType.Stone;
                        }
                        else if (worldY < Settings.OCEAN_HEIGHT && worldY > Utils.GenerateHeight(worldX, worldZ))
                        {
                            blockType = BlockType.Water;
                            addToFluid = true;
                        }
                        else blockType = BlockType.Air;


                        if ((worldY <= 1) && (worldY > 0)) blockType = BlockType.Bedrock;


                        chunkData[x, y, z] = new Block(blockType, position, addToFluid ? fluid : chunk, this);
                    }
                }
            }

            chunkState = ChunkState.Draw;
        }

        public void Redraw()
        {
            Object.DestroyImmediate(chunk.GetComponent<MeshFilter>());
            Object.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
            Object.DestroyImmediate(chunk.GetComponent<Collider>());

            Object.DestroyImmediate(fluid.GetComponent<MeshFilter>());
            Object.DestroyImmediate(fluid.GetComponent<MeshRenderer>());
            DrawChunk();
        }

        public void DrawChunk()
        {
            if (!_treesGenerated)
            {
                for (int z = 0; z < Settings.CHUNK_SIZE; z++)
                {
                    for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                    {
                        for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                        {
                            BuildTrees(chunkData[x, y, z], x, y, z);
                        }
                    }
                }

                _treesGenerated = true;
            }


            // Rendering
            for (int z = 0; z < Settings.CHUNK_SIZE; z++)
            {
                for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                {
                    for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                    {
                        chunkData[x, y, z].Draw();
                    }
                }
            }

            CombineQuads(chunk, _solidMaterial);
            MeshCollider meshCollider = chunk.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = chunk.GetComponent<MeshFilter>().mesh;

            CombineQuads(fluid, _fluidMaterial);
            chunkState = ChunkState.Idle;
        }

        private void CombineQuads(GameObject gObject, Material material)
        {
            // Combining
            MeshFilter[] meshFilters = gObject.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }

            // Creating new mesh
            MeshFilter meshFilter = gObject.GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = gObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();

            // Assign mesh
            meshFilter.mesh.CombineMeshes(combine);

            // Add renderer
            MeshRenderer meshRenderer = gObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            // Delete all children (the quads)
            gObject.transform.DestroyAllChildren();
        }

        private void BuildTrees(Block seed, int x, int y, int z)
        {
            if (seed.blockSetup.blockType != BlockType.TreeSeed) return;

            Block trunk = seed.GetBlock(x, y + 1, z);
            if (trunk != null)
            {
                trunk.SetParent(trunk.owner.chunk);
                trunk.SetType(BlockType.Wood);
                Block trunk1 = seed.GetBlock(x, y + 2, z);
                if (trunk1 != null)
                {
                    trunk1.SetParent(trunk1.owner.chunk);
                    trunk1.SetType(BlockType.Wood);

                    for (int i = -2; i <= 2; i++)
                    {
                        for (int j = -2; j <= 2; j++)
                        {
                            for (int k = 3; k <= 4; k++)
                            {
                                Block treetop = seed.GetBlock(x + i, y + k, z + j);
                                if (treetop != null)
                                {
                                    if (i == 0 && j == 0)
                                    {
                                        treetop.SetType(BlockType.Wood);
                                    }
                                    else
                                    {
                                        treetop.SetType(BlockType.Leaves);
                                    }
                                }
                                else return;
                            }
                        }
                    }

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Block treetop0 = seed.GetBlock(x + i, y + 5, z + j);
                            treetop0?.SetParent(treetop0.owner.chunk);
                            treetop0?.SetType(BlockType.Leaves);

                            if (Math.Abs(i) != Math.Abs(j) || (i == 0 && j == 0))
                            {
                                Block treetop1 = seed.GetBlock(x + i, y + 6, z + j);
                                treetop1?.SetParent(treetop1.owner.chunk);
                                treetop1?.SetType(BlockType.Leaves);
                            }
                        }
                    }
                }
            }
        }
    }
}