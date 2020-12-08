using Terrain.Utils;
using System;
using Terrain;
using TerrainGeneration.Analytics;

namespace Generation.Terrain.Procedural
{
    public class DiamondSquare : ITerrainModifier
    {
        protected Random RandomGenerator;
        protected float[,] Heightmap;

        public DiamondSquare(int seed = 0)
        {
            RandomGenerator = seed > 0 ? new Random(seed) : new Random();
        }

        public virtual void Apply(float[,] heightmap)
        {
            TimeLogger.Start(this.GetType().Name, heightmap.GetLength(0));
            
            Heightmap = heightmap;

            RandomizeCorners();

            float height = 1.0f;
            int numSquares = 1;
            int squareSize = heightmap.GetLength(0)-1;

            while (squareSize > 1)
            {
                int row = 0;
                for (int i = 0; i < numSquares; i++)
                {
                    int col = 0;
                    for (int j = 0; j < numSquares; j++)
                    {
                        DiamondSquareAlgorithm(row, col, squareSize, height);
                        col = (j + 1) * squareSize;     // equivalent to -> col += squareSize, but compatible with shader version
                    }
                    row = (i + 1) * squareSize;         // equivalent to -> row += squareSize, but compatible with shader version
                }
                numSquares *= 2;
                squareSize /= 2;
                height *= 0.5f;
            }

            TimeLogger.RecordSingleTimeInMilliseconds();
            Heightmap = Heightmap.Normalize();
        }

        private void DiamondSquareAlgorithm(int row, int col, int size, float offset)
        {
            int halfSize = size / 2;
            Coords topLeft = new Coords(col, row);
            Coords topRight = new Coords(col + size, row);
            Coords bottomLeft = new Coords(col, row + size);
            Coords bottomRight = new Coords(col + size, row + size);
            Coords mid = new Coords(col + halfSize, row + halfSize);

            Coords up = new Coords(topLeft.X + halfSize, topLeft.Y);
            Coords left = new Coords(mid.X - halfSize, mid.Y);
            Coords right = new Coords(mid.X + halfSize, mid.Y);
            Coords down = new Coords(bottomLeft.X + halfSize, bottomLeft.Y);

            // diamond step
            Heightmap[mid.X, mid.Y] = Average(Heightmap[topLeft.X, topLeft.Y], Heightmap[topRight.X, topRight.Y], Heightmap[bottomLeft.X, bottomLeft.Y], Heightmap[bottomRight.X, bottomRight.Y]) + RandomValue(offset);

            // square step
            Heightmap[up.X, up.Y] = Average(Heightmap[topLeft.X, topLeft.Y], Heightmap[topRight.X, topRight.Y], Heightmap[mid.X, mid.Y], GetHeight(up.X, up.Y + halfSize)) + RandomValue(offset);
            Heightmap[left.X, left.Y] = Average(Heightmap[topLeft.X, topLeft.Y], Heightmap[bottomLeft.X, bottomLeft.Y], Heightmap[mid.X, mid.Y], GetHeight(left.X - halfSize, left.Y)) + RandomValue(offset);
            Heightmap[right.X, right.Y] = Average(Heightmap[topRight.X, topRight.Y], Heightmap[bottomRight.X, bottomRight.Y], Heightmap[mid.X, mid.Y], GetHeight(right.X + halfSize, right.Y)) + RandomValue(offset);
            Heightmap[down.X, down.Y] = Average(Heightmap[bottomLeft.X, bottomLeft.Y], Heightmap[bottomRight.X, bottomRight.Y], Heightmap[mid.X, mid.Y], GetHeight(down.X, down.Y - halfSize)) + RandomValue(offset);
        }

        private float Average(float a, float b, float c, float d) => (d == 0.0f) ? (a + b + c) / 3.0f : (a + b + c + d) * 0.25f;

        private float GetHeight(Coords position) => GetHeight(position.X, position.Y);

        private float GetHeight(int x, int y) => IsOutOfLimits(x, y) ? 0.0f : Heightmap[x, y];

        private bool IsOutOfLimits(int x, int y) => x < 0 || x >= Heightmap.GetLength(0) || y < 0 || y >= Heightmap.GetLength(1);

        protected void RandomizeCorners()
        {
            Heightmap[0, 0] = RandomValue();
            Heightmap[0, Heightmap.GetLength(0)-1] = RandomValue();
            Heightmap[Heightmap.GetLength(0)-1, 0] = RandomValue();
            Heightmap[Heightmap.GetLength(0)-1, Heightmap.GetLength(0)-1] = RandomValue();
        }

        private float RandomValue(float range = 1.0f)
        {
            float min = -range;
            float max = range;
            float random = (float)RandomGenerator.NextDouble();
            
            return (max - min) * random + min;
        }
    }
}
