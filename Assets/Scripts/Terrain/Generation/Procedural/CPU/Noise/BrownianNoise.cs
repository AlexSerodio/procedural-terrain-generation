using Terrain;

namespace Generation.Terrain.Procedural.Noise
{
    public class BrownianNoise : ITerrainModifier
    {
        public float PerlinXScale { get; set; }
        public float PerlinYScale { get; set; }
        public int PerlinOffsetX { get; set; }
        public int PerlinOffsetY { get; set; }
        public int PerlinOctaves { get; set; }
        public float PerlinPersistance { get; set; }
        public float PerlinHeightScale { get; set; }

        private PerlinNoise perlinNoise;

        public BrownianNoise()
        {
            perlinNoise = new PerlinNoise();

            PerlinXScale = 0.01f;
            PerlinYScale = 0.01f;
            PerlinPersistance = 8;
            PerlinHeightScale = 0.09f;
        }

        public void Apply(float[,] heightmap)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightmap[x, y] += FractionalBrownianMotion(
                        (x + PerlinOffsetX) * PerlinXScale,
                        (y + PerlinOffsetY) * PerlinYScale,
                        PerlinOctaves, PerlinPersistance
                    ) * PerlinHeightScale;
                }
            }
        }

        /// <summary>
        /// Fractional Brownian Motion.
        /// Expands the Perlin Noise function adding several octaves together, with each octave 
        /// added being larger than the previous one, based on the 'persistence' factor.
        /// </summary>
        public float FractionalBrownianMotion(float x, float y, int octave, float persistance)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            for (int i = 0; i < octave; i++)
            {
                total += perlinNoise.Single(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistance;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}
