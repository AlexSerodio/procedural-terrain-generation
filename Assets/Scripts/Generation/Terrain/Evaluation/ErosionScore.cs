using Generation.Terrain.Utils;
using System.Collections.Generic;
using System;

namespace Generation.Terrain.Evaluation
{
    public static class ErosionScore
    {
        public static float Evaluate(float[,] heightmap)
        {
            float[,] slopemap = GetSlopemap(heightmap);
            
            float meanValue = GetMeanValue(slopemap);
            float standardDeviation = GetStandardDeviation(slopemap, meanValue);

            return standardDeviation / meanValue;
        }

        private static float[,] GetSlopemap(float[,] heightmap)
        {
            int xSize = heightmap.GetLength(0);
            int ySize = heightmap.GetLength(1);
            float[,] slopemap = new float[xSize, ySize];

            for (int x = 0; x < xSize; x++)
                for (int y = 0; y < ySize; y++)
                    slopemap[x, y] = GetGreatestSlopeBetweenNeighbors(x, y, heightmap);

            return slopemap;
        }

        private static float GetGreatestSlopeBetweenNeighbors(int x, int y, float[,] heightmap)
        {
            List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), heightmap.GetLength(0), heightmap.GetLength(1));
            float greatestSlope = float.MinValue;
            foreach (Coords neighbor in neighbors)
            {
                float slope = Math.Abs(heightmap[x, y] - heightmap[neighbor.X, neighbor.Y]);
                if (slope > greatestSlope)
                    greatestSlope = slope;
            }

            return greatestSlope;
        }

        private static float GetMeanValue(float[,] slopemap)
        {
            int amount = slopemap.Length;
            float total = 0.0f;

            for (int x = 0; x < slopemap.GetLength(0); x++)
                for (int y = 0; y < slopemap.GetLength(1); y++)
                    total += slopemap[x, y];
            
            return total / amount;
        }

        private static float GetStandardDeviation(float[,] slopemap, float meanValue)
        {
            int amount = slopemap.Length;
            float total = 0.0f;

            for (int x = 0; x < slopemap.GetLength(0); x++)
                for (int y = 0; y < slopemap.GetLength(1); y++)
                    total += (slopemap[x, y] - meanValue) * (slopemap[x, y] - meanValue);
            
            float variance = total / amount;
            return (float) Math.Sqrt(variance);
        }
    }
}
