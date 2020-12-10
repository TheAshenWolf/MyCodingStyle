using System;
using UnityEngine;

namespace VoxelWorld
{
    [Serializable]
    public enum CubeSide
    {
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }

    [Serializable]
    public enum BlockType
    {
        Air,
        Grass,
        Dirt,
        Stone,
        Planks,
        Brick,
        Wood,
        Bedrock,
        CoalOre,
        IronOre,
        GoldOre,
        RedstoneOre
    }

    [Serializable]
    public enum ChunkState
    {
        Idle,
        Keep,
        Draw
    }
    
}