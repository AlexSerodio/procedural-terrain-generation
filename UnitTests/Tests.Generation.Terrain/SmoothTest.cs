using FluentAssertions;
using Generation.Terrain.Utils;
using Xunit;
using Xunit.Abstractions;

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
            float[,] actualHeighmap = ReadHeightMap("heightmap-1");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-1-smoothed");

            // Act
            TerrainUtils.Smooth(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap2_ShouldBeEqualSmoothedHeighmap2()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-2");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-2-smoothed");

            // Act
            TerrainUtils.Smooth(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap3_ShouldBeEqualSmoothedHeighmap3()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-3");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-3-smoothed");

            // Act
            TerrainUtils.Smooth(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }

        [Fact]
        public void Smooth_WithHeightmap4_ShouldBeEqualSmoothedHeighmap4()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap("heightmap-4");
            float[,] expectedHeighmap = ReadHeightMap("heightmap-4-smoothed");

            // Act
            TerrainUtils.Smooth(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }
    }
}