using System.Collections.Generic;
using System.Numerics;

namespace Generation.Terrain.Utils
{
    public static class TerrainUtils
    {
        /// <summary>
        /// Smooth the heighMap informed. Each position gets the average height of its eight neighbors plus its own.
        /// </summary>
        /// <param name="heightMap">The heighMap to be smoothed.</param>
        /// <param name="iterations">The number of times the algorithm will be repeated.</param>
        /// <returns>The smoothed heightMap.</returns>
        public static float[,] Smooth(float[,] heightMap, int iterations = 1)
        {
            for (int i = 0; i < iterations; i++)
            {
                for (int x = 0; x < heightMap.GetLength(0); x++)
                {
                    for (int y = 0; y < heightMap.GetLength(1); y++)
                    {
                        float totalHeight = heightMap[x, y];
                        List<Vector2> neighbors = Neighborhood.Moore(new Vector2(x, y), heightMap.GetLength(0), heightMap.GetLength(1));
                        foreach (Vector2 neighbor in neighbors)
                            totalHeight += heightMap[(int)neighbor.X, (int)neighbor.Y];

                        heightMap[x, y] = totalHeight / (neighbors.Count + 1);
                    }
                }
            }
            return heightMap;
        }
    }
}