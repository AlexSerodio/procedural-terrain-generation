namespace Generation.Terrain.Procedural.Noise
{
    public class PerlinNoise : ITerrainModifier
    {
        public void Apply(float[,] heightmap)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    heightmap[x, y] = Single(x, y);
            }
        }

        // TODO: Implement own PerlinNoise function to not need UnityEngine anymore;
        public float Single(float x, float y)
        {
            return UnityEngine.Mathf.PerlinNoise(x, y);
        }
    }
}