using FluentAssertions;
using Generation.Terrain.Utils;
using System.Collections.Generic;
using System.Numerics;
using Xunit.Abstractions;
using Xunit;

namespace Tests.Generation.Terrain
{
    [Collection("MustReadFile")]
    public class VonNeumannNeighborhoodTest : BaseTest
    {
        private readonly float[,] Heightmap;
        private readonly int Width;
        private readonly int Height;

        public VonNeumannNeighborhoodTest(ITestOutputHelper output) : base(output)
        {
            Heightmap = ReadHeightMap("heightmap-1");
            Width = Heightmap.GetLength(0);
            Height = Heightmap.GetLength(1);
        }

        [Fact]
        public void GetHeightMapDimensions_ShouldReturn513x513()
        {
            // Arrange
            int expectedWidth = 513;
            int expectedHeight = 513;

            // Assert
            Width.Should().Be(expectedWidth);
            Height.Should().Be(expectedHeight);
        }

        [Fact]
        public void VonNeumann_FromSeedOnCenter_ShouldReturnFourNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = (Height-1) / 2;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(4);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnUpperLeftCorner_ShouldReturnTwoNeighbors()
        {
            // Arrange
            int x = 0;
            int y = 0;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(2);
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnUpperRightCorner_ShouldReturnTwoNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = 0;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(2);
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnUpperCenter_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = 0;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnBottomLeftCorner_ShouldReturnTwoNeighbors()
        {
            // Arrange
            int x = 0;
            int y = Height-1;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(2);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnBottomRightCorner_ShouldReturnTwoNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = Height-1;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(2);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnBottomCenter_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = Height-1;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnLeftCenter_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = 0;
            int y = (Height-1) / 2;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x+1, y));    // right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void VonNeumann_FromSeedOnRightCenter_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = (Height-1) / 2;
            Vector2 seed = new Vector2(x, y);

            // Act
            List<Vector2> neighbors = Neighborhood.VonNeumann(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Vector2(x, y-1));    // up
            neighbors.Should().Contain(new Vector2(x, y+1));    // down
            neighbors.Should().Contain(new Vector2(x-1, y));    // left
            neighbors.Should().NotContain(seed);
        }
    }
}
