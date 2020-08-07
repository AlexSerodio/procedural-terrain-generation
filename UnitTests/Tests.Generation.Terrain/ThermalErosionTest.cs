using FluentAssertions;
using Generation.Terrain.Utils;
using Simulation.Terrain.DiegoliNeto;
using Xunit;
using Xunit.Abstractions;

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

        [Fact]
        public void Erode_WithHeightmap2_ShouldBeEqualErodedHeighmap2()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap2.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap2.Eroded);

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }

        [Fact]
        public void Erode_WithHeightmap3_ShouldBeEqualErodedHeighmap3()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap3.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap3.Eroded);

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }

        [Fact]
        public void Erode_WithHeightmap4_ShouldBeEqualErodedHeighmap4()
        {
            // Arrange
            float[,] actualHeighmap = ReadHeightMap(Files.Heightmap4.Original);
            float[,] expectedHeighmap = ReadHeightMap(Files.Heightmap4.Eroded);

            // Act
            thermalErosion.Erode(actualHeighmap, talus, factor, iterations);

            // Assert
            AreEqual(actualHeighmap, expectedHeighmap).Should().Be(true);
        }
    }
}