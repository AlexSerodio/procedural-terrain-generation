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
                    sediment[x, y] += solubility * water[x, y];
                    heightmap[x, y] -= solubility * water[x, y];

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

                    float a = heightmap[x, y] + water[x, y];                 // Altura total atual.
                    float total = 0f;                                                   // Média das alturas das superfícies vizinhas
                    int count = 0;
                    float dTotal = 0f;                                                 // Soma das alturas de todos s vizinhos inferiores a posição atual

                    List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), maxX, maxY);

                    foreach (var neighbor in neighbors)
                    {
                        float ai = heightmap[neighbor.X, neighbor.Y] + water[neighbor.X, neighbor.Y];
                        float di = a - ai;
                        // Se a altura total atual for menor que a altura total do vizinho, skip 
                        if (di < 0)
                            continue;

                        dTotal += di;
                        total += ai;
                        count++;
                    }

                    // Se não houver diferença de altura, a água está estabilizada.
                    if (dTotal == 0)
                        return;

                    float avg = total / count;

                    foreach (var neighbor in neighbors)
                    {
                        float ai = heightmap[neighbor.X, neighbor.Y] + water[neighbor.X, neighbor.Y];
                        float di = a - ai;
                        if (di < 0)
                            continue;

                        // Fórmula de distribuição de água e sedimento.
                        float deltaW = Math.Min(water[x, y], (a - avg)) * (di / dTotal);
                        float deltaM = sediment[x, y] * (deltaW / water[x, y]);

                        water[neighbor.X, neighbor.Y] += deltaW;
                        sediment[neighbor.X, neighbor.Y] += deltaM;

                        water[x, y] -= deltaW;
                        sediment[x, y] -= deltaM;
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
                    float deltaM = Math.Max(0, sediment[x, y] - maxSediment);
                    
                    sediment[x, y] -= deltaM;
                    heightmap[x, y] += deltaM;

                    heightmap[x, y] = Math.Min(1f, heightmap[x, y]);
                }
            }
        }
    }
}
