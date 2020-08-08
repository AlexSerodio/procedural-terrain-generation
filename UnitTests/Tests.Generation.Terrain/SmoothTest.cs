using FluentAssertions;
using Generation.Terrain.Utils;
using Xunit.Abstractions;
using Xunit;

namespace Tests.Generation.Terrain
{
    [Collection("MustReadFile")]
    public class SmoothTest : BaseTest
    {
        private const int iterations = 10;
        private const float allowedDifference = 0.00002f;

        public SmoothTest(ITestOutputHelper output) 
            : base(output) { }

        [Fact]
        public void Smooth_WithHeightmap1_ShouldBeEqualSmoothedHeighmap1()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap1.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap1.Smoothed);

            // Act
            Smooth.Apply(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap2_ShouldBeEqualSmoothedHeighmap2()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap2.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap2.Smoothed);

            // Act
            Smooth.Apply(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap3_ShouldBeEqualSmoothedHeighmap3()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap3.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap3.Smoothed);

            // Act
            Smooth.Apply(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap4_ShouldBeEqualSmoothedHeighmap4()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap4.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap4.Smoothed);

            // Act
            Smooth.Apply(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }
    }
}