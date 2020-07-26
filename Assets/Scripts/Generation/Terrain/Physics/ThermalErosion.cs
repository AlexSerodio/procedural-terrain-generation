using System.Collections.Generic;
using System.Numerics;
using Generation.Terrain.Utils;

namespace Generation.Terrain.Physics.Erosion
{
    /// <summary>
    /// Foreach position, check if the height of it's neighbors is less than the current height plus he erosionStrength.
    /// If so, a percentage of the current position height is removed from the current position and added to the neighbor.
    /// </summary>
    public class ThermalErosion : Erosion
    {
        public float ErosionStrength { get; set; }
        public float ErosionAmount { get; set; }

        public ThermalErosion()
        {
            ErosionStrength = 0.1f;
            ErosionAmount = 0.01f;
        }

        public ThermalErosion(float erosionStrength, float erosionAmount)
        {
            ErosionStrength = erosionStrength;
            ErosionAmount = erosionAmount;
        }

        public float[,] Erode(float[,] heightMap)
        {
            int rows = heightMap.GetLength(0);
            int columns = heightMap.GetLength(1);
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector2 thisLocation = new Vector2(x, y);
                    List<Vector2> neighbors = TerrainUtils.GetMooreNeighbors(thisLocation, rows, columns);
                    
                    foreach (Vector2 neighbor in neighbors)
                    {
                        if (heightMap[x, y] > heightMap[(int)neighbor.X, (int)neighbor.Y] + ErosionStrength)
                        {
                            float currentHeight = heightMap[x, y];
                            heightMap[x, y] -= currentHeight * ErosionAmount;
                            heightMap[(int)neighbor.X, (int)neighbor.Y] += currentHeight * ErosionAmount;
                        }
                    }
                }
            }
            return heightMap;
        }
    }
}
