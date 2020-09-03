using Generation.Terrain.Utils;
using System;

namespace Generation.Terrain.Procedural
{
    public class DiamondSquare : ITerrainModifier
    {
        public int Resolution { get; set; }
        public float Height { get; set; }

        public DiamondSquare()
        {
            Height = 20;
        }

        public DiamondSquare(float height, int resolution)
        {
            Height = height;
            Resolution = resolution;
        }

        public void Apply(float[,] heightmap)
        {
            if (Resolution == 0)
                Resolution = heightmap.GetLength(0) - 1;

            RandomizeCorners(heightmap);

            int iterations = (int)Math.Log(Resolution, 2);
            int numSquares = 1;
            int squareSize = Resolution;

            for (int i = 0; i < iterations; i++)
            {
                int row = 0;
                for (int j = 0; j < numSquares; j++)
                {
                    int col = 0;
                    for (int k = 0; k < numSquares; k++)
                    {
                        DiamondSquareAlgorithm(row, col, squareSize, Height, heightmap);
                        col += squareSize;
                    }
                    row += squareSize;
                }
                numSquares *= 2;
                squareSize /= 2;
                Height *= 0.5f;
            }
        }

        private void RandomizeCorners(float[,] heightmap)
        {
            heightmap[0, 0] = RandomValue(Height);
            heightmap[0, Resolution] = RandomValue(Height);
            heightmap[Resolution, 0] = RandomValue(Height);
            heightmap[Resolution, Resolution] = RandomValue(Height);
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

        private float RandomValue(float range)
        {
            return UnityEngine.Random.Range(-range, range);
        }
    }
}