using System;

namespace Generation.Terrain.Evaluation
{
    public static class BenfordsLaw
    {
        public static string Evaluate(float[,] heightmap)
        {
            float[] percentages = new float[9];
            int total = heightmap.Length;

            for (int i = 0; i < heightmap.GetLength(0); i++)
            {
                for (int j = 0; j < heightmap.GetLength(1); j++)
                {
                    float scaledHeight = heightmap[i, j] * 100000000000000f;
                    scaledHeight = scaledHeight == 0 ? 1 : scaledHeight;
                    int firstDigit = Convert.ToInt32(scaledHeight.ToString().Substring(0, 1));

                    percentages[firstDigit-1]++;
                }
            }

            for (int i = 0; i < percentages.Length; i++)
                percentages[i] = (float)Math.Round((percentages[i] / total) * 100f);

            float sum = 0;
            for (int i = 0; i < percentages.Length; i++)
                sum += percentages[i];

            return string.Join(", ", percentages) + " -> " + sum;
        }
    }
}
