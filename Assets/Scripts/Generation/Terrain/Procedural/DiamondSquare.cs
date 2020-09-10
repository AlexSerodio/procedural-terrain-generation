using Generation.Terrain.Utils;

namespace Generation.Terrain.Procedural
{
    public class DiamondSquare : ITerrainModifier
    {
        public int Resolution { get; set; }

        public DiamondSquare() { }

        public DiamondSquare(int resolution)
        {
            Resolution = resolution;
        }

        public void Apply(float[,] heightmap)
        {
            if (Resolution == 0)
                Resolution = heightmap.GetLength(0) - 1;

            RandomizeCorners(heightmap);

            float height = 1.0f;
            int numSquares = 1;
            int squareSize = Resolution;

            while (squareSize > 1)
            {
                int row = 0;
                for (int j = 0; j < numSquares; j++)
                {
                    int col = 0;
                    for (int k = 0; k < numSquares; k++)
                    {
                        DiamondSquareAlgorithm(row, col, squareSize, height, heightmap);
                        col = (k + 1) * squareSize;     // equivalent to -> col += squareSize
                    }
                    row = (j + 1) * squareSize;         // equivalent to -> row += squareSize
                }
                numSquares *= 2;
                squareSize /= 2;
                height *= 0.5f;
            }
        }

        private void RandomizeCorners(float[,] heightmap)
        {
            heightmap[0, 0] = RandomValue();
            heightmap[0, Resolution] = RandomValue();
            heightmap[Resolution, 0] = RandomValue();
            heightmap[Resolution, Resolution] = RandomValue();
        }

        private void DiamondSquareAlgorithm(int row, int col, int size, float offset, float[,] heightmap)
        {
            int halfSize = (int)(size * 0.5f);
            Coords topLeft = new Coords(col, row);
            Coords topRight = new Coords(col + size, row);
            Coords bottomLeft = new Coords(col, row + size);
            Coords bottomRight = new Coords(col + size, row + size);
            Coords mid = new Coords(halfSize + col, halfSize + row);

            // diamond step
            heightmap[mid.X, mid.Y] = (heightmap[topLeft.X, topLeft.Y] + heightmap[topRight.X, topRight.Y] + heightmap[bottomLeft.X, bottomLeft.Y] + heightmap[bottomRight.X, bottomRight.Y]) * 0.25f + RandomValue(offset);

            // square step
            heightmap[topLeft.X + halfSize, topLeft.Y] = (heightmap[topLeft.X, topLeft.Y] + heightmap[topRight.X, topRight.Y] + heightmap[mid.X, mid.Y]) / 3 + RandomValue(offset);
            heightmap[mid.X - halfSize, mid.Y] = (heightmap[topLeft.X, topLeft.Y] + heightmap[bottomLeft.X, bottomLeft.Y] + heightmap[mid.X, mid.Y]) / 3 + RandomValue(offset);
            heightmap[mid.X + halfSize, mid.Y] = (heightmap[topRight.X, topRight.Y] + heightmap[bottomRight.X, bottomRight.Y] + heightmap[mid.X, mid.Y]) / 3 + RandomValue(offset);
            heightmap[bottomLeft.X + halfSize, bottomLeft.Y] = (heightmap[bottomLeft.X, bottomLeft.Y] + heightmap[bottomRight.X, bottomRight.Y] + heightmap[mid.X, mid.Y]) / 3 + RandomValue(offset);
        }

        private float RandomValue(float range = 1.0f)
        {
            return UnityEngine.Random.Range(-range, range);
        }
    }
}
