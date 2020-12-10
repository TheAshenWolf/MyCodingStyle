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
            blockMatrix = new BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
            for (int z = 0; z < World.chunkSize; z++)
            {
                for (int y = 0; y < World.chunkSize; y++)
                {
                    for (int x = 0; x < World.chunkSize; x++)
                    {
                        blockMatrix[x, y, z] = blocks[x, y, z].blockType;
                    }
                }
            }
        }
    }
}