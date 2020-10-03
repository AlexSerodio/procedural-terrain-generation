using Generation.Terrain.Utils;
using System;

namespace Generation.Terrain.Procedural
{
    public class DiamondSquare : ITerrainModifier
    {
        public int Resolution { get; set; }

        protected Random random;
        protected float[,] Heightmap;

        public DiamondSquare(int resolution, int seed = int.MinValue)
        {
            Resolution = resolution;
            random = seed > int.MinValue ? new Random(seed) : new Random();
        }

        public virtual void Apply(float[,] heightmap)
        {
            Heightmap = heightmap;

            if (Resolution == 0)
                Resolution = heightmap.GetLength(0) - 1;

            RandomizeCorners();

            float height = 1.0f;
            int numSquares = 1;
            int squareSize = Resolution;

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
            Heightmap[mid.X, mid.Y] = Average(GetHeight(topLeft), GetHeight(topRight), GetHeight(bottomLeft), GetHeight(bottomRight)) + RandomValue(offset);

            // square step
            Heightmap[up.X, up.Y] = Average(GetHeight(topLeft), GetHeight(topRight), GetHeight(mid), GetHeight(up.X, up.Y + halfSize)) + RandomValue(offset);
            Heightmap[left.X, left.Y] = Average(GetHeight(topLeft), GetHeight(bottomLeft), GetHeight(mid), GetHeight(left.X - halfSize, left.Y)) + RandomValue(offset);
            Heightmap[right.X, right.Y] = Average(GetHeight(topRight), GetHeight(bottomRight), GetHeight(mid), GetHeight(right.X + halfSize, right.Y)) + RandomValue(offset);
            Heightmap[down.X, down.Y] = Average(GetHeight(bottomLeft), GetHeight(bottomRight), GetHeight(mid), GetHeight(down.X, down.Y - halfSize)) + RandomValue(offset);
        }

        private float Average(float a, float b, float c, float d) => (d == 0.0f) ? (a + b + c) / 3.0f : (a + b + c + d) * 0.25f;

        private float GetHeight(Coords position) => GetHeight(position.X, position.Y);

        private float GetHeight(int x, int y) => IsOutOfLimits(x, y) ? 0.0f : Heightmap[x, y];

        private bool IsOutOfLimits(int x, int y) => x < 0 || x >= Heightmap.GetLength(0) || y < 0 || y >= Heightmap.GetLength(1);

        protected void RandomizeCorners()
        {
            Heightmap[0, 0] = RandomValue();
            Heightmap[0, Resolution] = RandomValue();
            Heightmap[Resolution, 0] = RandomValue();
            Heightmap[Resolution, Resolution] = RandomValue();
        }

        private float RandomValue(float range = 1.0f)
        {
            float max = range;
            float min = -range;
            float x = (float)random.NextDouble();
            
            return (max - min) * x + min;
        }
    }
}
