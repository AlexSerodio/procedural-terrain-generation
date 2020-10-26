using System;
using System.Collections.Generic;
using Generation.Terrain.Utils;

namespace Generation.Terrain.Physics.Erosion
{
    public class HydraulicErosionDiegoliOptimized : HydraulicErosionDiegoli
    {
        public override void Erode(float[,] heightmap, float pour, float solubility, float evaporationFactor, int iterations)
        {
            float[,] water = heightmap.Clone() as float[,];

            for (int i = 0; i < iterations; i++)
                Erode(heightmap, water, pour, solubility, evaporationFactor);
        }

        public override  void Erode(float[,] heightmap, float[,] water, float rainFactor, float solubility, float evaporationFactor)
        {
            PourWaterAndDissolve(heightmap, water, rainFactor, solubility);
            WaterFlow(heightmap, water);
            DrainWater(heightmap, water, solubility, evaporationFactor);
        }

        private void PourWaterAndDissolve(float[,] heightmap, float[,] water, float rainFactor, float solubility)
        {
            for (int x = 0; x < heightmap.GetLength(0); x++)
            {
                for (int y = 0; y < heightmap.GetLength(1); y++)
                {
                    // Pour Water
                    water[x, y] += rainFactor;
                    
                    // Dissolve
                    if(solubility <= 0f)
                        continue;
                    
                    float waterVolume = water[x, y] - heightmap[x, y];
                    if (waterVolume <= 0)
                        continue;

                    float amountToRemove = solubility * waterVolume;
                    heightmap[x, y] -= amountToRemove;
                    heightmap[x, y] = Math.Max(0f, heightmap[x, y]);
                }
            }
        }

        private void WaterFlow(float[,] heightmap, float[,] water)
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    float localSurfaceHeight = water[x, y];
                    float localWaterVolume = localSurfaceHeight - heightmap[x, y];

                    if (localWaterVolume < 0)
                        continue;

                    float avgSurfaceHeight = 0;
                    int countHeights = 0;
                    float totalDifference = 0;

                    List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), maxX, maxY);

                    foreach (var neighbor in neighbors)
                    {
                        float nearbySurfaceHeight = water[neighbor.X, neighbor.Y];
                        float difference = localSurfaceHeight - nearbySurfaceHeight;
                        
                        if (difference < 0)
                            continue;

                        totalDifference += difference;
                        avgSurfaceHeight += nearbySurfaceHeight;
                        countHeights++;
                    }

                    if (totalDifference == 0)
                        continue;

                    avgSurfaceHeight /= countHeights;

                    float deltaSurfaceHeight = localSurfaceHeight - avgSurfaceHeight;

                    float totalDeltaWater = 0;

                    foreach (var neighbor in neighbors)
                    {
                        float nearbySurfaceHeight = water[neighbor.X, neighbor.Y];
                        
                        if (nearbySurfaceHeight >= localSurfaceHeight) 
                            continue;
                        
                        float difference = localSurfaceHeight - nearbySurfaceHeight;
                        float deltaWater = Math.Min(localWaterVolume, deltaSurfaceHeight) * (difference / totalDifference);

                        water[neighbor.X, neighbor.Y] += deltaWater;
                        totalDeltaWater += deltaWater;
                    }

                    if (totalDeltaWater > 0)
                        water[x, y] -= totalDeltaWater;
                }
            }
        }

        private void DrainWater(float[,] heightmap, float[,] water, float solubility, float evaporationFactor)
        {
            if (evaporationFactor == 0)
                return;

            float evaporationPercent = (1f - evaporationFactor);

            for (int x = 0; x < heightmap.GetLength(0); x++)
            {
                for (int y = 0; y < heightmap.GetLength(1); y++)
                {
                    float waterVolume = water[x, y] - heightmap[x, y];

                    if (waterVolume <= 0) 
                        continue;

                    float delta = waterVolume - (waterVolume * evaporationPercent);
                    water[x, y] -= delta;
                    heightmap[x, y] += solubility * delta;
                    heightmap[x, y] = Math.Min(1f, heightmap[x, y]);
                }
            }
        }
    }
}
