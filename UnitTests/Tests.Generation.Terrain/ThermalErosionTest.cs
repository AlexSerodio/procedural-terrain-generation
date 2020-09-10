using FluentAssertions;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Utils;
using Xunit.Abstractions;
using Xunit;

namespace Tests.Generation.Terrain
{
    [Collection("MustReadFile")]
    public class ThermalErosionTest : BaseTest
    {
        private ThermalErosion thermalErosion;

        private const int N = 513;
        private const float talus = 1f / N;     // our talus value is smaller than Olsen's (4/N) possibly because we are working with greater height differences.
        private const float factor = 0.5f;
        private const int iterations = 500;

        public ThermalErosionTest(ITestOutputHelper output) : base(output)
        {
            thermalErosion = new ThermalErosion();
        }

        [Theory]
        [InlineData(Files.Heightmap2.Original, Files.Heightmap2.Eroded)]
        [InlineData(Files.Heightmap3.Original, Files.Heightmap3.Eroded)]
        [InlineData(Files.Heightmap4.Original, Files.Heightmap4.Eroded)]
        public void Erode_ThermalErosion_ShouldBeEqualErodedHeighmap(string originalHeightmap, string erodedHeightmap)
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(originalHeightmap);
            float[,] expectedHeighmap = ReadHeightMap(erodedHeightmap);

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }
    }
}
