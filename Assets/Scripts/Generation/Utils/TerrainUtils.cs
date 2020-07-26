using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Generation.Utils
{
    public static class TerrainUtils
    {
        /// <summary>
        /// Retrieves all eight neighbors from the position informed, following Moore's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore 
        /// will not be returned.
        /// </summary>
        /// <param name="position">The central position from which wants to recover the neighbors.</param>
        /// <param name="xMax">The maximum X position.</param>
        /// <param name="yMax">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Vector2> GetMooreNeighbors(Vector2 position, int xMax, int yMax)
        {
            HashSet<Vector2> neighbors = new HashSet<Vector2>();
            Vector2 min = Vector2.Zero;
            Vector2 max = new Vector2(xMax-1, yMax-1);
            
            neighbors.Add(Vector2.Clamp(new Vector2(position.X-1, position.Y+1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X, position.Y+1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X+1, position.Y+1), min, max));

            neighbors.Add(Vector2.Clamp(new Vector2(position.X-1, position.Y), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X+1, position.Y), min, max));
            
            neighbors.Add(Vector2.Clamp(new Vector2(position.X-1, position.Y-1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X, position.Y-1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X+1, position.Y-1), min, max));
            
            return neighbors.ToList();
        }

        /// <summary>
        /// Retrieves all four neighbors from the position informed, following Von Neumann's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore
        /// will not be returned.
        /// </summary>
        /// <param name="position">The central position from which wants to recover the neighbors.</param>
        /// <param name="xMax">The maximum X position.</param>
        /// <param name="yMax">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Vector2> GetVonNeumannNeighbors(Vector2 position, int width, int height)
        {
            HashSet<Vector2> neighbors = new HashSet<Vector2>();
            Vector2 min = Vector2.Zero;
            Vector2 max = new Vector2(width-1, height-1);
            
            neighbors.Add(Vector2.Clamp(new Vector2(position.X, position.Y+1), min, max));

            neighbors.Add(Vector2.Clamp(new Vector2(position.X-1, position.Y), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(position.X+1, position.Y), min, max));
            
            neighbors.Add(Vector2.Clamp(new Vector2(position.X, position.Y-1), min, max));
            
            return neighbors.ToList();
        }

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
                        List<Vector2> neighbors = GetMooreNeighbors(new Vector2(x, y), heightMap.GetLength(0), heightMap.GetLength(1));
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