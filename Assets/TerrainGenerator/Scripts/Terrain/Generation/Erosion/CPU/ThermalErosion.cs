﻿using Terrain.Utils;
using System.Collections.Generic;
using System;
using Terrain;
using Terrain.Generation.Configurations;
using TerrainGeneration.Analytics;

namespace Generation.Terrain.Physics.Erosion
{
    /// <summary>
    /// Fornece algoritmos de erosão térmica de terrenos detalhados por Olsen (2004).
    /// Os algoritmos apresentados nesta classe (assim como algumas das descrições existentes nos algoritmos) 
    /// foram implementados por Guilherme Diegoli Neto durante seu Trabalho de Conclusão de Curso (TCC)
    /// do curso de Ciência da Computação da universidade Fundação Universidade Regional de Blumenau (FURB).
    /// 
    /// Algumas alterações foram realizadas nos algoritmos originais.
    /// O código original está disponível em sua monografia, nas páginas 35 à 40.
    /// 
    /// O trabalho em questão está disponível em: http://dsc.inf.furb.br/tcc/index.php?cd=6&tcc=1867.
    /// </summary>
    public class ThermalErosion : ITerrainModifier
    {
        public ThermalErosionConfig Config { get; }

        public ThermalErosion(ThermalErosionConfig configuration)
        {
            Config = configuration;
        }

        /// <summary>
        /// Applies the DryErosion algorithm multiple times to get more realistic results.
        /// The amount of repetitions is controlled by the 'iterations' variable.
        /// </summary>
        public virtual void Apply(float[,] heightmap)
        {
            TimeLogger.Start(this.GetType().Name, heightmap.GetLength(0));
            
            for (int i = 0; i < Config.Iterations; i++)
                Erode(heightmap, Config.Talus, Config.Strength);
            
            TimeLogger.RecordSingleTimeInMilliseconds();
        }

        /// <summary>
        /// Simula os efeitos de erosão térmica, que causa a queda de materiais em encostas de montanhas,
        /// tornando a encosta mais íngreme e gerando acumulo de materiais na sua base. 
        /// 
        /// O algoritmo atinge esse efeito através da movimentação de material de pontos mais altos para vizinhos 
        /// mais baixos quando a diferença entre os dois ultrapassa um determinado valor. A quantidade de material 
        /// a ser movimentada é inicialmente obtida através da fórmula (factor * (heightDiff - talus)), onde factor 
        /// representa um fator limitante de movimentação, heightDiff representa a diferença entre as alturas, 
        /// e talus representa a diferença máxima permitida.
        /// Olsen (2004, p. 6) sugere que o valor ideal para factor é 0,5.
        /// </summary>
        /// <param name="heightmap">Mapa de altura do relevo.</param>
        /// <param name="talus">Diferença de altura máxima permitida.</param>
        /// <param name="strength">Fator limitante de movimentação.</param>
        private void Erode(float[,] heightmap, float talus, float strength)
        {
            int maxX = heightmap.GetLength(0);
            int maxY = heightmap.GetLength(1);
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    float maxHeightDiff = float.MinValue;   // Maior diferença encontrada entre os vizinhos.
                    float sumExceededDiffs = 0;             // Soma das diferenças que ultrapassam o limite talus.

                    List<Coords> neighbors = Neighborhood.VonNeumann(new Coords(x, y), maxX, maxY);

                    // Primeiro loop não realiza alteração no relevo.
                    // Percorre os vizinhos calculando qual a maior diferença de altura entre todos
                    // e a soma de todas as diferenças de altura que ultrapassam o limite talus.
                    foreach (var neighbor in neighbors)
                    {
                        float heightDiff = heightmap[x, y] - heightmap[neighbor.X, neighbor.Y];
                        if (heightDiff > maxHeightDiff)
                            maxHeightDiff = heightDiff;
                        if (heightDiff > talus)
                            sumExceededDiffs += heightDiff;
                    }

                    // Se não existir nenhuma diferença de altura que ultrapasse o limite, o ponto está estabilizado.
                    if (sumExceededDiffs == 0)
                        continue;

                    // Segundo loop é onde são feitas as alterações.
                    foreach (var neighbor in neighbors)
                    {
                        float heightDiff = heightmap[x, y] - heightmap[neighbor.X, neighbor.Y];

                        // Se a diferença de altura entre a posição atual e algum vizinho for maior que o limite talus,
                        // remove material da posição atual e aplica ao vizinho.
                        if (heightDiff > talus)
                        {
                            // Fórmula de distribuição do solo.
                            float sediment = strength * (maxHeightDiff - talus) * (heightDiff / sumExceededDiffs);
                            heightmap[neighbor.X, neighbor.Y] += sediment;
                            heightmap[x, y] -= sediment;

                            heightmap[neighbor.X, neighbor.Y] = Math.Min(1f, heightmap[neighbor.X, neighbor.Y]);
                            heightmap[x, y] = Math.Max(0f, heightmap[x, y]);
                        }
                    }
                }
            }
        }
    }
}
