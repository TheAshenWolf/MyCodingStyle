using System;

namespace VoxelWorld
{
    [Serializable]
    public class BlockData
    {
        public BlockType[,,] blockMatrix;

        public BlockData()
        {
        }

        public BlockData(Block[,,] blocks)
        {
            blockMatrix = new BlockType[Settings.CHUNK_SIZE, Settings.CHUNK_SIZE, Settings.CHUNK_SIZE];
            for (int z = 0; z < Settings.CHUNK_SIZE; z++)
            {
                for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                {
                    for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                    {
                        blockMatrix[x, y, z] = blocks[x, y, z].blockSetup.blockType;
                    }
                }
            }
        }
    }
}