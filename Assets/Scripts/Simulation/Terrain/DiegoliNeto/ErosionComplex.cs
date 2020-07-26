using System;

namespace Simulation.Terrain.DiegoliNeto
{
    public class ErosionComplex
    {
        float[] inclinationMods = { 1.0f, 1.1f, 1.2f, 2.0f };
        public void DryErosion(float[,] soil, float[,] rock, int[,] surface, float[,] humidity, float talus, float factor)
        {
            for (int x = 0; x < soil.GetLength(0); x++)
            {
                for (int y = 0; y < soil.GetLength(1); y++)
                {
                    float soilVolume = (soil[x, y] - rock[x, y]);
                    float maxDiff = 0;
                    float sumDiff = 0;
                    float sumDelta = 0;
                    float localTalus = talus * inclinationMods[surface[x, y]];
                    localTalus -= (humidity[x, y] * talus) / 2;

                    // Primeiro loop é necessário para totalizar as informações
                    // necessárias para calcular as alterações
                    Neighborhood.VonNeumann(x, y, soil,
                        (int relX, int relY) => {
                            float diff = soil[x, y] - soil[relX, relY];
                            if (diff > maxDiff)
                                maxDiff = diff;
                            if (diff > localTalus) {
                                sumDiff += diff;
                                sumDelta += factor * (diff - localTalus);
                            }
                        }
                    );
                    
                    // Se este valor for zero, o ponto está estabilizado
                    if (sumDiff == 0)
                        continue;
                    
                    // Limitar a quantidade de material ao volume do solo
                    float limiter = 1.0f;
                    if (sumDelta > soilVolume)
                        limiter = soilVolume / sumDelta;
                    
                    float inclinationDifference = (maxDiff - talus);
                    
                    // Segundo loop é onde são feitas as alterações
                    Neighborhood.VonNeumann(x, y, soil,
                        (int relX, int relY) => {
                            float diff = soil[x, y] - soil[relX, relY];
                            if (diff > localTalus)
                            {
                                float move = factor * (maxDiff - localTalus) * (diff / sumDiff) * limiter;
                                soil[relX, relY] += move;
                                soil[x, y] -= move;
                                
                                // Locais que recebem queda de material têm sua
                                // superfície destruída
                                surface[relX, relY] = 0;
                            }
                        }
                    );
                }
            }
        }

        private void WaterFlow(float [,] soil, float[,] water)
        {
            for (int x = 0; x < soil.GetLength(0); x++)
            {
                for (int y = 0; y < soil.GetLength(1); y++)
                {
                    if (water[x, y] < 0)
                        continue;
                    
                    float surface = soil[x, y] + water[x, y];
                    float avg = 0;
                    int count = 0;
                    float sumDiff = 0;
                    bool skip = false;
                    Neighborhood.VonNeumann(x, y, soil,
                        (int relX, int relY) => {
                            float localSurface = soil[relX, relY] + water[relX, relY];
                            float diff = surface - localSurface;
                            if (diff < 0)
                            {
                                skip = true;
                            } 
                            else {
                                sumDiff += diff;
                                avg += localSurface;
                                count++;
                            }
                        }
                    );

                    if (skip)
                        continue;
                    
                    // Se este valor for zero, a água está estabilizada
                    if (sumDiff == 0)
                        continue;
                    
                    avg /= count;
                    Neighborhood.VonNeumann(x, y, soil,
                        (int relX, int relY) => {
                            float localSurface = soil[relX, relY] + water[relX, relY];
                            float diff = surface - localSurface;
                            if (diff >= 0)
                            {
                                float delta = Math.Min(water[x, y], (surface - avg)) * (diff / sumDiff);
                                water[relX, relY] += delta;
                                water[x, y] -= delta;
                            }
                        }
                    );
                }
            }
        }

        float[] pourMods = { 1.0f, 0.8f, 0.4f, 1.0f };
        // Algoritmo de precipitação
        private void PourWater(float[,] water, int[,] surface, float pour)
        {
            // 'surface' são os tipos de solo (4 tipos), sendo que cada um possui uma taxa de vazão diferente ('pourMods')
            for (int x = 0; x < water.GetLength(0); x++)
            {
                for (int y = 0; y < water.GetLength(1); y++)
                {
                    water[x, y] += pour * pourMods[surface[x, y]];
                }
            }
        }

        // Algoritmo de remoção do solo
        private void Dissolve(float[,] soil, float[,] rock, float[,] water, float solubility)
        {
            for (int x = 0; x < soil.GetLength(0); x++)
            {
                for (int y = 0; y < soil.GetLength(1); y++)
                {
                    float delta = solubility * water[x, y];
                    // diminui do delta da altura do solo apenas se ainda tiver solo?
                    soil[x, y] -= Math.Min(delta, soil[x, y] - rock[x, y]);
                }
            }
        }

        float[] drainMods = { 1.0f, 0.8f, 0.8f, 0.0f };
        // Algoritmo de drenagem
        private void DrainWater(float[,] soil, float[,] water, int[,] surface, float[,] humidity, float solubility, float drain, float waterVolume)
        {
            // 'surface' são os tipos de solo (4 tipos), sendo que cada um possui uma taxa de vazão diferente ('pourMods')
            for (int x = 0; x < soil.GetLength(0); x++)
            {
                for (int y = 0; y < soil.GetLength(1); y++)
                {
                    float delta = waterVolume - (water[x, y] * drain);
                    delta *= drainMods[surface[x, y]];
                    water[x, y] -= delta;
                    soil [x, y] += solubility * delta;
                    humidity[x, y] += delta;
                    
                    // Limitar a umidade máxima a 1
                    if (humidity[x, y] > 1)
                        humidity [x, y] = 1;
                }
            }
        }
    }
}