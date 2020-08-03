namespace Simulation.Terrain.DiegoliNeto
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
    public class ThermalErosion
    {
        /// <summary>
        /// Simula os efeitos de erosão térmica, que causa a queda de materiais em encostas de montanhas,
        /// tornando a encosta mais íngreme e gerando acumulo de materiais na sua base. 
        /// 
        /// O algoritmo atinge esse efeito através da movimentação de material de pontos mais altos para vizinhos 
        /// mais baixos quando a diferença entre os dois ultrapasse um determinado valor. A quantidade de material 
        /// a ser movimentada é inicialmente obtida através da fórmula (factor * (heightDiff - talus)), onde factor 
        /// representa um fator limitante de movimentação, heightDiff representa a diferença entre as alturas, 
        /// e talus representa a diferença máxima permitida.
        /// Olsen (2004, p. 6) sugere que o valor ideal para factor é 0,5.
        /// </summary>
        /// <param name="matrix">Mapa de altura do relevo.</param>
        /// <param name="talus">Diferença de altura máxima permitida.</param>
        /// <param name="factor">Fator limitante de movimentação.</param>
        public void DryErosion(float[,] matrix, float talus = 1, float factor = 0.5f)
        {
            int maxX = matrix.GetLength(0);
            int maxY = matrix.GetLength(1);
            int count = 0;
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    float maxHeightDiff = 0;            // Maior diferença encontrada entre os vizinhos.
                    float sumExceededDiffs = 0;         // Soma das diferenças que ultrapassam o limite talus.

                    // Primeiro loop não realiza nenhuma alteração no no relevo.
                    // Percorre os vizinhos calculando qual a maior diferença de altura entre todos
                    // e a soma de todas as diferenças de altura que ultrapassam o limite talus.
                    
                    Neighborhood.VonNeumann(x, y, matrix, (int relX, int relY) => {
                        float heightDiff = matrix[x, y] - matrix[relX, relY];
                        if (heightDiff > maxHeightDiff)
                            maxHeightDiff = heightDiff;
                        if (heightDiff > talus)
                            sumExceededDiffs += heightDiff;
                    });
                    
                    // Se não existir nenhuma diferença de altura que ultrapasse o limite, o ponto está estabilizado.
                    if (sumExceededDiffs == 0)
                        continue;
                    
                    // float inclinationDifference = (maxHeightDiff - talus);
                    
                    // Segundo loop é onde são feitas as alterações.
                    Neighborhood.VonNeumann(x, y, matrix, (int relX, int relY) => {
                        float heightDiff = matrix[x, y] - matrix[relX, relY];
                        
                        count++;
                        // Se a diferença de altura entre a posição atual e algum vizinho for maior que o limite talus,
                        // remove material da posição atual e aplica ao vizinho.
                        if (heightDiff > talus)
                        {
                            // Fórmula de distribuição do solo.
                            // float move = factor * (maxHeightDiff - talus) * (heightDiff / sumExceededDiffs);
                            float move = (factor * (heightDiff - talus));
                            matrix[relX, relY] += move;
                            matrix[x, y] -= move;
                        }
                    });
                }
            }
        }
    }
}