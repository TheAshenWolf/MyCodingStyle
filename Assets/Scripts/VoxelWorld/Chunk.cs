using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TheAshenWolf;
using UnityEngine;

namespace VoxelWorld
{
    public class Chunk
    {
        public Material material;
        public Block[,,] chunkData;
        public GameObject chunk;
        public ChunkState chunkState;
        public BlockData blockData;
        public string chunkName;
        public bool changed;

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
                blockData = new BlockData();
                blockData = (BlockData) binaryFormatter.Deserialize(file);
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
                Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
            blockData = new BlockData(chunkData);
            binaryFormatter.Serialize(file, blockData);
            file.Close();

            changed = false;
        }

        public Chunk(Vector3 position, Material material)
        {
            chunkName = World.BuildChunkName(position);
            chunk = new GameObject(chunkName);
            chunk.transform.position = position + new Vector3(0.5f, 0.5f, 0.5f);
            this.material = material;
            BuildChunk();
        }


        public void BuildChunk()
        {
            bool loadingFromFile = false;
            loadingFromFile = Load();

            chunkData = new Block[Settings.CHUNK_SIZE, Settings.CHUNK_SIZE, Settings.CHUNK_SIZE];

            for (int z = 0; z < Settings.CHUNK_SIZE; z++)
            {
                for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                {
                    for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        int worldX = (int) (x + chunk.transform.position.x);
                        int worldY = (int) (y + chunk.transform.position.y);
                        int worldZ = (int) (z + chunk.transform.position.z);

                        if (loadingFromFile)
                        {
                            chunkData[x, y, z] = new Block(blockData.blockMatrix[x, y, z], position, chunk.gameObject,
                                this);
                            continue;
                        }

                        BlockType blockType;

                        // Default "chunk" of materials grass - dirt - stone - bedrock
                        if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, pers: 0.04f) < .4f)
                            blockType = BlockType.Air;
                        else if (worldY == Utils.GenerateHeight(worldX, worldZ)) blockType = BlockType.Grass;
                        else if (worldY < Utils.GenerateHeight(worldX, worldZ) &&
                                 worldY > Utils.GenerateStoneHeight(worldX, worldZ)) blockType = BlockType.Dirt;
                        else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ) && worldY > 1)
                        {
                            if (worldY < Utils.maxHeight / 4 * 3 &&
                                Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.3f, 2, .15f) < .376f)
                            {
                                blockType = BlockType.CoalOre;
                            }
                            else if (worldY < Utils.maxHeight / 8f * 2.5f &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.25f, 3, .29f) < .37f)
                            {
                                blockType = BlockType.IronOre;
                            }
                            else if (worldY < Utils.maxHeight / 16f * 1.75f &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.2f, 2, .22f) < .35f)
                            {
                                blockType = BlockType.GoldOre;
                            }
                            else if (worldY < 20 &&
                                     Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, 0.19f, 3, .16f) < .35f)
                            {
                                blockType = BlockType.RedstoneOre;
                            }


                            else blockType = BlockType.Stone;
                        }
                        else blockType = BlockType.Air;

                        if ((worldY <= 1) && (worldY > 0)) blockType = BlockType.Bedrock;


                        chunkData[x, y, z] = new Block(blockType, position, chunk, this);
                    }
                }
            }

            chunkState = ChunkState.Draw;
        }

        public void Redraw()
        {
            GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
            GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
            GameObject.DestroyImmediate(chunk.GetComponent<Collider>());
            DrawChunk();
        }

        public void DrawChunk()
        {
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

            CombineQuads();

            MeshCollider meshCollider = chunk.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
            chunkState = ChunkState.Idle;
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