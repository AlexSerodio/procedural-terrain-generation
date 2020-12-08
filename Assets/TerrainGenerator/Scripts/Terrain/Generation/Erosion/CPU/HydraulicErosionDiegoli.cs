using System.Collections.Generic;
using System;
using Terrain.Generation.Configurations;
using Terrain.Utils;
using Terrain;
using TerrainGeneration.Analytics;

namespace Generation.Terrain.Physics.Erosion
{
    /// <summary>
    /// Fornece algoritmos de erosão hidráulica de terrenos detalhados por Olsen (2004).
    /// 
    /// Erosão hidráulica causa o deslocamento de material causado pela precipitação e escoamento da água.
    /// 
    /// Os algoritmos apresentados nesta classe (assim como algumas das descrições existentes nos algoritmos) 
    /// foram implementados por Guilherme Diegoli Neto durante seu Trabalho de Conclusão de Curso (TCC)
    /// do curso de Ciência da Computação da universidade Fundação Universidade Regional de Blumenau (FURB).
    /// 
    /// Algumas alterações foram realizadas nos algoritmos originais. 
    /// O código original está disponível em sua monografia, nas páginas 35 à 40.
    /// 
    /// O trabalho em questão está disponível em: http://dsc.inf.furb.br/tcc/index.php?cd=6&tcc=1867.
    /// <remarks>
    /// Os algoritmos presentes aqui parecem precisar serem executados em uma ordem especifica. 
    /// Sendo a ordem:
    ///     1. PourWater
    ///     3. Dissolve
    ///     2. WaterFlow
    ///     4. DrainWater
    /// </remarks>
    /// </summary>
    public class HydraulicErosionDiegoli : ITerrainModifier
    {
        public HydraulicErosionConfig Config { get; }

        public HydraulicErosionDiegoli(HydraulicErosionConfig configuration)
        {
            Config = configuration;
        }

        public virtual void Apply(float[,] heightmap)
        {
            TimeLogger.Start(this.GetType().Name, heightmap.GetLength(0));
            
            float[,] water = heightmap.Clone() as float[,];

            for (int i = 0; i < Config.Iterations; i++)
            {
                PourWater(water, Config.Rain);
                Dissolve(heightmap, water, Config.Solubility);
                WaterFlow(heightmap, water);
                DrainWater(heightmap, water, Config.Solubility, Config.Evaporation);
            }

            TimeLogger.RecordSingleTimeInMilliseconds();
        }

        /// <summary>
        /// Simula o processo de precipitação (chuva) através da soma de um valor de precipitação 
        /// pour a todos os valores em water.
        /// </summary>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="rainFactor">Valor de precipitação (quantidade de água adicionada).</param>
        private void PourWater(float[,] water, float rainFactor)
        {
            for (int x = 0; x < water.GetLength(0); x++)
                for (int y = 0; y < water.GetLength(1); y++)
                    water[x, y] += rainFactor;
        }

        /// <summary>
        /// Simula o escoamento da água através de um algoritmo similar ao utilizada na erosão térmica.
        /// Porém, diferentemente do algoritmo de erosão térmica, visa nivelar completamente as
        /// diferenças de alturas ao invés de se basear em um limite.
        /// </summary>
        /// <param name="heightmap">Mapa de altura do relevo.</param>
        /// <param name="water">Mapa de volumes de água para simular a hidrografia do relevo. Não considera a altura do relevo.</param>
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
                            return;
                        
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

        /// <summary>
        /// Simula o processo de drenagem de água do solo através da subtração de um 
        /// percentual drain de cada valor em water.
        /// </summary>
        /// <param name="heightmap">Mapa de altura do relevo</param>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="solubility">Fator de solubilidade do solo. Controla a quantidade de solo que será dissolvido.</param>
        /// <param name="evaporationFactor">Fator de drenagem da água do solo.</param>
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

        /// <summary>
        /// Algoritmo de remoção do solo.
        /// Simula a erosão hidráulica do solo por meio da subtração de uma parcela proporcional 
        /// à quantia de água presente sobre cada célula dos valores de altura em matrix, de acordo 
        /// com um fator de solubilidade.
        /// </summary>
        /// <param name="heightmap">Mapa de altura do relevo</param>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="solubility">Fator de solubilidade do solo. Controla a quantidade de solo que será dissolvido.</param>
        private void Dissolve(float[,] heightmap, float[,] water, float solubility)
        {
            if(solubility <= 0f)
                return;

            for (int x = 0; x < heightmap.GetLength(0); x++)
            {
                for (int y = 0; y < heightmap.GetLength(1); y++)
                {
                    float waterVolume = water[x, y] - heightmap[x, y];
                    if (waterVolume <= 0)
                        continue;

                    float amountToRemove = solubility * waterVolume;
                    heightmap[x, y] -= amountToRemove;
                    heightmap[x, y] = Math.Max(0f, heightmap[x, y]);
                }

            }
        }
    }
}
