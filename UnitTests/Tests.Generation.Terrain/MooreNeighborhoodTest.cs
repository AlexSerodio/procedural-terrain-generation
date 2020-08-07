using FluentAssertions;
using Generation.Terrain.Utils;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit;

namespace Tests.Generation.Terrain
{
    [Collection("MustReadFile")]
    public class MooreNeighborhoodTest : BaseTest
    {
        private readonly float[,] Heightmap;
        private readonly int Width;
        private readonly int Height;

        public MooreNeighborhoodTest(ITestOutputHelper output) : base(output)
        {
            Heightmap = ReadHeightMap(Files.Heightmap1.Original);
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
        public void Moore_FromSeedOnCenter_ShouldReturnEightNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = (Height-1) / 2;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(8);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x-1, y-1));  // upper left
            neighbors.Should().Contain(new Coords(x+1, y-1));  // upper right
            neighbors.Should().Contain(new Coords(x-1, y+1));  // bottom left
            neighbors.Should().Contain(new Coords(x+1, y+1));  // bottom right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnUpperLeftCorner_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = 0;
            int y = 0;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x+1, y+1));  // bottom right

            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnUpperRightCorner_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = 0;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x-1, y+1));  // bottom left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnUpperCenter_ShouldReturnFiveNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = 0;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(5);
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x-1, y+1));  // bottom left
            neighbors.Should().Contain(new Coords(x+1, y+1));  // bottom right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnBottomLeftCorner_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = 0;
            int y = Height-1;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x+1, y-1));  // upper right
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnBottomRightCorner_ShouldReturnThreeNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = Height-1;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(3);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x-1, y-1));  // upper left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnBottomCenter_ShouldReturnFiveNeighbors()
        {
            // Arrange
            int x = (Width-1) / 2;
            int y = Height-1;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(5);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x+1, y-1));  // upper right
            neighbors.Should().Contain(new Coords(x-1, y-1));  // upper left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnLeftCenter_ShouldReturnFiveNeighbors()
        {
            // Arrange
            int x = 0;
            int y = (Height-1) / 2;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(5);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x+1, y));    // right
            neighbors.Should().Contain(new Coords(x+1, y+1));  // bottom left
            neighbors.Should().Contain(new Coords(x+1, y-1));  // upper left
            neighbors.Should().NotContain(seed);
        }

        [Fact]
        public void Moore_FromSeedOnRightCenter_ShouldReturnFiveNeighbors()
        {
            // Arrange
            int x = Width-1;
            int y = (Height-1) / 2;
            Coords seed = new Coords(x, y);

            // Act
            List<Coords> neighbors = Neighborhood.Moore(seed, Width, Height);

            // Assert
            neighbors.Should().HaveCount(5);
            neighbors.Should().Contain(new Coords(x, y-1));    // up
            neighbors.Should().Contain(new Coords(x, y+1));    // down
            neighbors.Should().Contain(new Coords(x-1, y));    // left
            neighbors.Should().Contain(new Coords(x-1, y+1));  // bottom right
            neighbors.Should().Contain(new Coords(x-1, y-1));  // upper left
            neighbors.Should().NotContain(seed);
        }
    }
}
