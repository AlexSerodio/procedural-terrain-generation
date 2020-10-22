using Generation.Terrain.Utils;
using System.Collections.Generic;
using System;

namespace Generation.Terrain.Physics.Erosion
{
    public class HydraulicErosion
    {
        private float[,] heightmap;
        private float[,] water;
        private float[,] sediment;

        public virtual void Erode(float[,] heightmap, float rainFactor, float solubility, float evaporationFactor, float sedimentCapacity, int iterations)
        {
            this.heightmap = heightmap;
            water = new float[this.heightmap.GetLength(0), this.heightmap.GetLength(1)];
            sediment = new float[this.heightmap.GetLength(0), this.heightmap.GetLength(1)];

            for (int i = 0; i < iterations; i++)
                Erode(rainFactor, solubility, evaporationFactor, sedimentCapacity);
        }

        public virtual void Erode(float rainFactor, float solubility, float evaporationFactor, float sedimentCapacity)
        {
            Rain(rainFactor);
            DissolveMaterial(solubility);
            MoveWaterAndSediment();
            Evaporation(evaporationFactor, sedimentCapacity);
        }

        // Step 1
        private void Rain(float rainFactor)
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);
            
            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    water[x, y] += rainFactor;
        }

        // Step 2
        private void DissolveMaterial(float solubility)
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    heightmap[x, y] -= solubility * water[x, y];
                    sediment[x, y] += solubility * water[x, y];

                    heightmap[x, y] = Math.Max(0f, heightmap[x, y]);
                }
            }
        }

        // Step 3
        private void MoveWaterAndSediment()
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (water[x, y] < 0)
                        return;

                    float a = heightmap[x, y] + water[x, y];
                    float aTotal = 0f;
                    int count = 0;
                    float dTotal = 0f;

                    List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), maxX, maxY);

                    foreach (var neighbor in neighbors)
                    {
                        float ai = heightmap[neighbor.X, neighbor.Y] + water[neighbor.X, neighbor.Y];
                        float di = a - ai;

                        dTotal += di;
                        aTotal += ai;
                        count++;
                    }

                    if (dTotal == 0)
                        return;

                    float average = aTotal / count;
                    float deltaA = a - average;
                    float minVal = Math.Min(water[x, y], deltaA);

                    foreach (var neighbor in neighbors)
                    {
                        float ai = heightmap[neighbor.X, neighbor.Y] + water[neighbor.X, neighbor.Y];
                        float di = a - ai;
                        
                        float deltaW = minVal * di / dTotal;
                        water[x, y] -= deltaW;
                        water[neighbor.X, neighbor.Y] += deltaW;

                        float deltaM = sediment[x, y] * deltaW / water[x, y];
                        sediment[x, y] -= deltaM;
                        sediment[neighbor.X, neighbor.Y] += deltaM;
                    }
                }
            }
        }

        // Step 4
        private void Evaporation(float evaporationFactor, float sedimentCapacity)
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    water[x, y] = water[x, y] * (1f - evaporationFactor);

                    float maxSediment = sedimentCapacity * water[x, y];
                    float deltaS = Math.Max(0, sediment[x, y] - maxSediment);
                    
                    sediment[x, y] -= deltaS;
                    heightmap[x, y] += deltaS;
                }
            }
        }
    }
}
