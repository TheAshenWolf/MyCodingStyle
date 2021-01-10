using UnityEngine;

namespace VoxelWorld
{
    public static class Utils
    {
        public const int MAX_HEIGHT = 128;
        private const float SMOOTH = 0.01f;
        private const int OCTAVES = 4;
        private const float PERSISTENCE = 0.5f;

        public static int GenerateHeight(float x, float z)
        {
            float height = Map(0, MAX_HEIGHT, 0, 1, FractalBrownianMotion(x * SMOOTH, z * SMOOTH, OCTAVES, PERSISTENCE));
            return (int) height;
        }

        public static int GenerateStoneHeight(float x, float z)
        {
            float height = Map(0, MAX_HEIGHT - 10, 0, 1,
                FractalBrownianMotion(x * SMOOTH, z * SMOOTH, OCTAVES, PERSISTENCE * .9f));
            return (int) height;
        }

        private static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
        {
            return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
        }

        public static float FractalBrownianMotion3D(float x, float y, float z, float sm = 0.1f, int oct = 3, float persistence = 0.5f)
        {
            float xy = FractalBrownianMotion(x * sm, y * sm, oct, persistence);
            float yz = FractalBrownianMotion(y * sm, z * sm, oct, persistence);
            float xz = FractalBrownianMotion(x * sm, z * sm, oct, persistence);

            float yx = FractalBrownianMotion(y * sm, x * sm, oct, persistence);
            float zy = FractalBrownianMotion(z * sm, y * sm, oct, persistence);
            float zx = FractalBrownianMotion(z * sm, x * sm, oct, persistence);

            return (xy + yz + xz + yx + zy + zx) / 6f;
        }

        private static float FractalBrownianMotion(float x, float z, int octaves, float persistence)
        {

            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            float offset = 32000f;

            for (int i = 0; i < octaves; i++)
            {
                total +=  Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}