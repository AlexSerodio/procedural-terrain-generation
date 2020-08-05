using System;
using FluentAssertions;
using Simulation.Terrain.DiegoliNeto;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Generation.Terrain
{
    public class ThermalErosionTest : BaseTest
    {
        private ThermalErosion thermalErosion;

        private const int N = 513;
        private const float talus = 1f / N;       // nosso talus é menor do que o do Olsen (4/N) possivelmente por estarmos trabalhando com diferenças de alturas maiores.
        private const float factor = 0.5f;
        private const int iterations = 500;

        public ThermalErosionTest(ITestOutputHelper output) : base(output)
        {
            thermalErosion = new ThermalErosion();
        }

        [Fact]
        public void Erode_WithHeightmap2_ShouldBeEqualErodedHeighmap2()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-2");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-2-eroded");

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }

        [Fact]
        public void Erode_WithHeightmap3_ShouldBeEqualErodedHeighmap3()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-3");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-3-eroded");

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }

        [Fact]
        public void Erode_WithHeightmap4_ShouldBeEqualErodedHeighmap4()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-4");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-4-eroded");

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }

        /// <summary>
        /// Compares whether the values from first matrix are equal the values from the second one.
        /// Float values can vary slightly from one matrix to another. The 'allowedDifference'
        /// parameter determines how much that difference can be.
        /// </summary>
        private bool AreEqual(float[,] actual, float[,] expected, float allowedDifference = 0.00015f)
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