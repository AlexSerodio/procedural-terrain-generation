using FluentAssertions;
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
    }
}