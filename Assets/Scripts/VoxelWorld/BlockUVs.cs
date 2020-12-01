using UnityEngine;

namespace VoxelWorld
{
    public static class BlockUVs
    {
        private static Vector2 GetUvCoords (int a, int b)
        {
            return new Vector2(0.0625f * a, 1 - 0.0625f * b);
        }

        public static Vector2[] GetUvsFromTexturePosition(int x, int y)
        {
            Vector2[] uvs =  new Vector2[4];

            uvs[0] = GetUvCoords(x + 1 , y);
            uvs[1] = GetUvCoords(x, y);
            uvs[2] = GetUvCoords(x , y + 1);
            uvs[3] = GetUvCoords(x + 1, y +1);

            return uvs;
        }

        public static Vector2[] GrassTop => GetUvsFromTexturePosition(2, 9); // Actually a green wool, but it looks better than the white grass
        public static Vector2[] Stone => GetUvsFromTexturePosition(1, 0);
        public static Vector2[] Dirt => GetUvsFromTexturePosition(2, 0);
        public static Vector2[] GrassSide => GetUvsFromTexturePosition(3, 0);
        public static Vector2[] Planks => GetUvsFromTexturePosition(4, 0);
        public static Vector2[] Brick => GetUvsFromTexturePosition(7, 0);

        public static Vector2[] WoodHorizontal => GetUvsFromTexturePosition(4, 1);
        public static Vector2[] WoodVertical => GetUvsFromTexturePosition(5, 1);
    }
}