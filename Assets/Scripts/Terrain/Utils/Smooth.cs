using System.Collections.Generic;

namespace Terrain.Utils
{
    /// <summary>
    /// Implements a heightmap smoothing algorithm.
    /// </summary>
    public static class Smooth
    {
        /// <summary>
        /// Smooths the heighmap passed as parameter. 
        /// Each position gets the average height of its eight neighbors plus its own.
        /// </summary>
        /// <param name="heightmap">The heighmap to be smoothed.</param>
        /// <param name="iterations">The number of times the algorithm will be repeated.</param>
        /// <returns>The smoothed heightmap.</returns>
        public static float[,] Apply(float[,] heightmap, int iterations = 1)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);

            for (int i = 0; i < iterations; i++)
                BoxBlur(heightmap, width, height);

            return heightmap;
        }

        private static void BoxBlur(float[,] matrix, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float totalHeight = matrix[x, y];

                    List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), width, height);
                    foreach (Coords neighbor in neighbors)
                        totalHeight += matrix[neighbor.X, neighbor.Y];

                    matrix[x, y] = totalHeight / (neighbors.Count + 1);
                }
            }
        }
    }
}
