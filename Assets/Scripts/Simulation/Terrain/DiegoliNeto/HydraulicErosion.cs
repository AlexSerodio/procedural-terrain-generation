using System;

namespace Simulation.Terrain.DiegoliNeto
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
    ///     2. WaterFlow
    ///     3. Dissolve
    ///     4. DrainWater
    /// 
    /// TODO: Talvez o correto seja 1, 3, 2, 4.
    /// </remarks>
    /// </summary>
    public class HydraulicErosion
    {
        /// <summary>
        /// Simula o processo de precipitação (chuva) através da soma de um valor de precipitação 
        /// pour a todos os valores em water.
        /// </summary>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="pour">Valor de precipitação (quantidade de água adicionada).</param>
        private void PourWater(float[,] water, float pour)
        {
            for (int x = 0; x < water.GetLength(0); x++)
            {
                for (int y = 0; y < water.GetLength(1); y++)
                {
                    water[x, y] += pour;
                }
            }
        }

        /// <summary>
        /// Simula o escoamento da água através de um algoritmo similar ao utilizada na erosão térmica.
        /// Porém, diferentemente do algoritmo de erosão térmica, visa nivelar completamente as
        /// diferenças de alturas ao invés de se basear em um limite.
        /// </summary>
        /// <param name="matrix">Mapa de altura do relevo.</param>
        /// <param name="water">Mapa de volumes de água para simular a hidrografia do relevo. Não considera a altura do relevo.</param>
        private void WaterFlow(float[,] matrix, float[,] water)
        {
            int maxX = matrix.GetLength(0);
            int maxY = matrix.GetLength(1);
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (water[x, y] < 0)
                        continue;
                    
                    float surface = matrix[x, y] + water[x, y];                         // Altura total atual.
                    float avg = 0;                                                      // Média das alturas das superfícies vizinhas
                    int count = 0;
                    float sumDiff = 0;                                                  // Soma das alturas de todos s vizinhos inferiores a posição atual
                    bool skip = false;

                    Neighborhood.VonNeumann(x, y, matrix, (int relX, int relY) => {
                        float localSurface = matrix[relX, relY] + water[relX, relY];    // Altura total do vizinho.
                        float diff = surface - localSurface;                            // Diferença entre atual e vizinho.
                        // Se a altura total atual for menor que a altura total do vizinho, skip 
                        if (diff < 0)           // TODO: alterar essa linha para if (surface < localSurface) quando houver testes
                            skip = true;
                        else
                        {
                            sumDiff += diff;
                            avg += localSurface;
                            count++;
                        }
                        
                    });
                    
                    if (skip)
                        continue; 

                    // Se não haver nenhuma diferença de altura, a água está estabilizada.
                    if (sumDiff == 0)
                        continue; 
 
                    avg /= count;
                    
                    Neighborhood.VonNeumann(x, y, matrix, (int relX, int relY) => {
                        float localSurface = matrix[relX, relY] + water[relX, relY];    // Altura total do vizinho.
                        float diff = surface - localSurface;                            // Diferença entre altura atual e vizinho.
                        if (diff >= 0)
                        {
                            // Fórmula de distribuição de água.
                            float delta = Math.Min(water[x, y], (surface - avg)) * (diff / sumDiff);
                            water[relX, relY] += delta;
                            water[x, y] -= delta;
                        }
                    });
                }
            }
        } 
 
        // Algoritmo de remoção do solo
        /// <summary>
        ///  Simula a erosão hidráulica do solo por meio da subtração de uma parcela proporcional 
        /// à quantia de água presente sobre cada célula dos valores de altura em matrix, de acordo 
        /// com um fator de solubilidade.
        /// </summary>
        /// <param name="matrix">Mapa de altura do relevo</param>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="solubility">Fator de solubilidade do solo. Controla a quantidade de solo que será dissolvido.</param>
        private void Dissolve(float[,] matrix, float[,] water, float solubility)
        {
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    matrix[x, y] -= solubility * water[x, y];
                }
            }
        } 
 
        /// <summary>
        /// Simula o processo de drenagem de água do solo através da subtração de um 
        /// percentual drain de cada valor em water.
        /// </summary>
        /// <param name="matrix">Mapa de altura do relevo</param>
        /// <param name="water">Mapa de volumes de água. Não considera a altura do relevo.</param>
        /// <param name="solubility">Fator de solubilidade do solo. Controla a quantidade de solo que será dissolvido.</param>
        /// <param name="drain">Fator de drenagem da água do solo.</param>
        /// <param name="waterVolume">Não sei ao certo. Não era um parâmetro mas é utilizado apenas neste método então o adicionei.</param>
        private void DrainWater(float[,] matrix, float[,] water, float solubility, float drain, float waterVolume)
        {
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    // TODO: o que é waterVolume? Só uma constante qualquer? O que exatamente representa?
                    float delta = waterVolume - (water[x, y] * drain);
                    water[x, y] -= delta;                   // Remove a água da posição.
                    matrix[x, y] += solubility * delta;     // Adiciona sedimentos no solo trazidos pela água.
                }
            }
        } 
    }
}