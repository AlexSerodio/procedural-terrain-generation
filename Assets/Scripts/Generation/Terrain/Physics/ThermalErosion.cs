using System.Collections.Generic;
// using System.Numerics;
using Generation.Terrain.Utils;
using UnityEngine;

namespace Generation.Terrain.Physics.Erosion
{
    /// <summary>
    /// Foreach position, check if the height of it's neighbors is less than the current height plus he erosionStrength.
    /// If so, a percentage of the current position height is removed from the current position and added to the neighbor.
    /// </summary>
    public class ThermalErosion : Erosion
    {
        public float ErosionStrength { get; }
        public float ErosionFactor { get; }

        public ThermalErosion(float erosionStrength, float erosionFactor)
        {
            ErosionStrength = erosionStrength;
            ErosionFactor = erosionFactor;
        }

        public float[,] Erode(float[,] heightMap)
        {
            int rows = heightMap.GetLength(0);
            int columns = heightMap.GetLength(1);
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    Vector2 thisLocation = new Vector2(x, y);
                    List<Vector2> neighbors = GenerateNeighbors(thisLocation, rows, columns);
                    
                    foreach (Vector2 neighbor in neighbors)
                    {
                        if (heightMap[x, y] > heightMap[(int)neighbor.x, (int)neighbor.y] + ErosionStrength)
                        {
                            float sediments = heightMap[x, y] * ErosionFactor;
                            heightMap[x, y] -= sediments;
                            heightMap[(int)neighbor.x, (int)neighbor.y] += sediments;
                        }
                    }
                }
            }
            return heightMap;
        }

        private List<Vector2> GenerateNeighbors(Vector2 pos, int width, int height)
        {
            List<Vector2> neighbors = new List<Vector2>();
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                    Mathf.Clamp(pos.y + y, 0, height - 1));
                        if (!neighbors.Contains(nPos))
                            neighbors.Add(nPos);
                    }
                }
            }
            return neighbors;
        }
    }
}
