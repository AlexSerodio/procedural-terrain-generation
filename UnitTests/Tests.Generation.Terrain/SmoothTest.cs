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

        [Theory]
        [InlineData(Files.Heightmap1.Original, Files.Heightmap1.Smoothed)]
        [InlineData(Files.Heightmap2.Original, Files.Heightmap2.Smoothed)]
        [InlineData(Files.Heightmap3.Original, Files.Heightmap3.Smoothed)]
        [InlineData(Files.Heightmap4.Original, Files.Heightmap4.Smoothed)]
        public void Smooth_ShouldBeEqualSmoothedHeighmap(string originalHeightmap, string smoothedHeightmap)
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(originalHeightmap);
            float[,] expectedHeighmap = ReadHeightMap(smoothedHeightmap);

            // Act
            Smooth.Apply(actualHeighmap, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap, allowedDifference).Should().Be(true);
        }
    }
}