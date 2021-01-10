using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace VoxelWorld
{
    public static class BlockUVs
    {
        private static Vector2 GetUvCoords(int a, int b)
        {
            return new Vector2(0.0625f * a, 1 - 0.0625f * b);
        }

        private static Vector2[] GetUvsFromTexturePosition(int x, int y)
        {
            Vector2[] uvs = new Vector2[4];

            uvs[0] = GetUvCoords(x + 1, y);
            uvs[1] = GetUvCoords(x, y);
            uvs[2] = GetUvCoords(x, y + 1);
            uvs[3] = GetUvCoords(x + 1, y + 1);

            return uvs;
        }

        public static Vector2[] GrassTop =>
            GetUvsFromTexturePosition(0,
                0); // GetUvsFromTexturePosition(2, 9); // Actually a light green wool, but it looks better than the white grass, I need to figure out how to color it though

        public static Vector2[] Stone => GetUvsFromTexturePosition(1, 0);
        public static Vector2[] Dirt => GetUvsFromTexturePosition(2, 0);
        public static Vector2[] GrassSide => GetUvsFromTexturePosition(3, 0);
        public static Vector2[] Planks => GetUvsFromTexturePosition(4, 0);
        public static Vector2[] StoneSlabHorizontal => GetUvsFromTexturePosition(5, 0);
        public static Vector2[] StoneSlabVertical => GetUvsFromTexturePosition(6, 0);
        public static Vector2[] Brick => GetUvsFromTexturePosition(7, 0);
        public static Vector2[] TntSide => GetUvsFromTexturePosition(8, 0);
        public static Vector2[] TntTop => GetUvsFromTexturePosition(9, 0);
        public static Vector2[] TntBottom => GetUvsFromTexturePosition(10, 0);
        public static Vector2[] Cobblestone => GetUvsFromTexturePosition(0, 1);
        public static Vector2[] Bedrock => GetUvsFromTexturePosition(1, 1);
        public static Vector2[] Sand => GetUvsFromTexturePosition(2, 1);
        public static Vector2[] Gravel => GetUvsFromTexturePosition(3, 1);
        public static Vector2[] WoodHorizontal => GetUvsFromTexturePosition(4, 1);
        public static Vector2[] WoodVertical => GetUvsFromTexturePosition(5, 1);
        public static Vector2[] IronBlock => GetUvsFromTexturePosition(6, 1);
        public static Vector2[] GoldBlock => GetUvsFromTexturePosition(7, 1);
        public static Vector2[] DiamondBlock => GetUvsFromTexturePosition(8, 1);

        public static Vector2[] ChestVertical => GetUvsFromTexturePosition(9, 1);
        public static Vector2[] ChestSide => GetUvsFromTexturePosition(10, 1);
        public static Vector2[] ChestFront => GetUvsFromTexturePosition(11, 1);

        public static Vector2[] GoldOre => GetUvsFromTexturePosition(0, 2);
        public static Vector2[] IronOre => GetUvsFromTexturePosition(1, 2);
        public static Vector2[] CoalOre => GetUvsFromTexturePosition(2, 2);
        
        public static Vector2[] RedstoneOre=> GetUvsFromTexturePosition(3, 3);
        public static Vector2[] BookshelfSide => GetUvsFromTexturePosition(3, 2);
        public static Vector2[] MossyCobble => GetUvsFromTexturePosition(4, 2);
        public static Vector2[] Obsidian => GetUvsFromTexturePosition(5, 2);

        public static Vector2[] Water => GetUvsFromTexturePosition(14, 0);

        public static Vector2[] Leaves => GetUvsFromTexturePosition(5, 3);
        
        
        
        // CRACKS
        
        public static Vector2[] GetCrack(Crack crack)
        {
            return crack == Crack.Crack0 ? GetUvsFromTexturePosition(5, 11) : GetUvsFromTexturePosition((int)crack - 1, 15);
        }
    }
}