using System;

namespace Simulation.Terrain.DiegoliNeto
{
    public class Neighborhood
    {
        /// <summary>
        /// Aplica para os quatro vizinhos de (x, y) as operações de transformação passadas
        /// através da função Transform(x, y).
        /// </summary>
        public static void VonNeumann (int x, int y, float[,] matrix, Action<int, int> Transform)
        {
            // "Transform" representa as operações de transformação da matriz
            if (x != 0)
                Transform((x - 1), y);
            
            if (y != 0)
                Transform(x, (y - 1));
            
            if (x != matrix.GetLength(0) - 1)
                Transform((x + 1), y);
            
            if (y != matrix.GetLength(1) - 1)
                Transform(x, (y + 1));
        }
    }
}