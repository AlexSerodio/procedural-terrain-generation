using System;
using Generation.Terrain.Utils;
using Xunit.Abstractions;

namespace Tests.Generation.Terrain
{
    public abstract class BaseTest
    {
        // protected static float[,] Heightmap = ReadHeightMap("heightmap-1");
        // protected int Width;
        // protected int Height;

        protected readonly ITestOutputHelper Output;

        public BaseTest(ITestOutputHelper output)
        {
            // Width = Heightmap.GetLength(0);
            // Height = Heightmap.GetLength(1);

            Output = output;
        }

        protected static float[,] ReadHeightMap(string filename)
        {
            var fullPath = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\Heighmaps\\";
            var reader = new ReadWriteTerrain(filename, fullPath);
            
            return reader.ReadMatrix();
        }

        /// <summary>
        /// Compares whether the values from first matrix are equal the values from the second one.
        /// Float values can vary slightly from one matrix to another. The 'allowedDifference'
        /// parameter determines how much that difference can be.
        /// </summary>
        protected bool AreEqual(float[,] actual, float[,] expected, float allowedDifference = 0.0002f)
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