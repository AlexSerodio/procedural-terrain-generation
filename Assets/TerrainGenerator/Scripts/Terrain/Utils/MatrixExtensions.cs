namespace Terrain.Utils
{
    public static class MatrixExtensions
    {
        public static float[,] Normalize(this float[,] matrix)
        {
            (float max, float min) = matrix.Limits();

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(0); j++)
                    matrix[i, j] = (matrix[i, j] - min) / (max - min);
            
            return matrix;
        }

        public static (float max, float min) Limits(this float[,] matrix)
        {
            float max = float.MinValue;
            float min = float.MaxValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    max = (matrix[i, j] > max) ? matrix[i, j] : max;
                    min = (matrix[i, j] < min) ? matrix[i, j] : min;
                }
            }
            
            return (max, min);
        } 
    }
}
