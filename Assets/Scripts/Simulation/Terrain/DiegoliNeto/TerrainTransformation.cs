namespace Simulation.Terrain.DiegoliNeto
{
    /// <summary>
    /// Algoritmos de transformação de relevo apresentados na seção 3.3.1.1 (p. 30) do TCC do Diegoli Neto.
    /// </summary>
    public class TerrainTransformation
    {
        // Algoritmo de smooth simples. Utilizar vizinhança de Moore (8 vizinhos).
        private void BoxBlur(float[,] matrix)
        {
            int maxX = matrix.GetLength(0);
            int maxY = matrix.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    float sum = 0;
                    int count = 0;

                    // Estes dois for parecem ser utilizados apenas para conseguir os vizinhos.
                    // Poderiam ser extraídos para um método a parte.
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= maxX)
                            continue;

                        for (int relY = -1; relY <= 1; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= maxY)
                                continue;
                            
                            sum += matrix[absX, absY];
                            count++;
                        }
                    }
                    if (count > 0)
                        matrix[x, y] = sum / count;
                }
            }
        }

        public void WindDecay(float[,] matrix)
        {
            int maxX = matrix.GetLength(0);
            int maxY = matrix.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y <maxY; y++)
                {
                    float sum = 0;
                    int count = 0;

                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= maxX)
                            continue;

                        // Considerar apenas vizinhos em coordenadas Y iguais ou
                        // inferiores ao ponto central
                        for (int relY = -1; relY <= 0; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= maxY)
                                continue;
                            
                            sum += matrix[absX, absY];
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        // Apenas efetuar a alteração caso o novo valor seja
                        // inferior ao atual
                        float avg = sum / count;
                        if (avg < matrix[x, y])
                            matrix[x, y] = avg;
                    }
                }
            }
        }
    }
}