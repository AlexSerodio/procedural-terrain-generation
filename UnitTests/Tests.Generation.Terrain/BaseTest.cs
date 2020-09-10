using Generation.Terrain.Utils;
using System;
using Xunit.Abstractions;

namespace Tests.Generation.Terrain
{
    public abstract class BaseTest
    {
        protected Files Files;
        protected readonly ITestOutputHelper Output;

        protected BaseTest(ITestOutputHelper output)
        {
            Output = output;
        }

        protected static float[,] ReadHeightMap(string filename)
        {
            return HeightmapSerializer.Deserialize(filename);
        }

        /// <summary>
        /// Compares whether the values from first matrix are equal the values from the second one.
        /// Float values can vary slightly from one matrix to another. The 'allowedDifference'
        /// parameter determines how much that difference can be.
        /// </summary>
        protected bool AreEqual(float[,] actual, float[,] expected, float allowedDifference = 0.00015f)
        {
            if(actual.GetLength(0) != expected.GetLength(0) || actual.GetLength(1) != expected.GetLength(1))
                return false;

            for (int i = 0; i < actual.GetLength(0); i++)
            {
                for (int j = 0; j < actual.GetLength(1); j++)
                {
                    if(Math.Abs(actual[i, j] - expected[i, j]) > allowedDifference)
                    {
                        Output.WriteLine("Actual: " + actual[i, j]);
                        Output.WriteLine("Expected: " + expected[i, j]);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
