using UnityEngine;

namespace VoxelWorld
{
    public class Utils
    {
        public static int maxHeight = 128;
        private static float smooth = 0.01f;
        private static int octaves = 4;
        private static float persistance = 0.5f;

        public static int GenerateHeight(float x, float z)
        {
            float height = Map(0, maxHeight, 0, 1, FractalBrownianMotion(x * smooth, z * smooth, octaves, persistance));
            return (int) height;
        }

        public static int GenerateStoneHeight(float x, float z)
        {
            float height = Map(0, maxHeight - 10, 0, 1,
                FractalBrownianMotion(x * smooth, z * smooth, octaves, persistance * .9f));
            return (int) height;
        }

        private static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
        {
            return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
        }

        public static float FractalBrownianMotion3D(float x, float y, float z, float sm = 0.1f, int oct = 3, float pers = 0.5f)
        {
            float xy = FractalBrownianMotion(x * sm, y * sm, oct, pers);
            float yz = FractalBrownianMotion(y * sm, z * sm, oct, pers);
            float xz = FractalBrownianMotion(x * sm, z * sm, oct, pers);

            float yx = FractalBrownianMotion(y * sm, x * sm, oct, pers);
            float zy = FractalBrownianMotion(z * sm, y * sm, oct, pers);
            float zx = FractalBrownianMotion(z * sm, x * sm, oct, pers);

            return (xy + yz + xz + yx + zy + zx) / 6f;
        }

        private static float FractalBrownianMotion(float x, float z, int octaves, float persistance)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            float offset = 32000f;

            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistance;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}