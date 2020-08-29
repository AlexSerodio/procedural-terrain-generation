using UnityEngine;

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
            if(Resolution == 0)
                Resolution = heightmap.GetLength(0)-1;

            RandomizeCorners(heightmap);

            int iterations = (int)Mathf.Log(Resolution, 2);
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
            heightmap[0, 0] = Random.Range(-Height, Height);
            heightmap[0, Resolution] = Random.Range(-Height, Height);
            heightmap[Resolution, 0] = Random.Range(-Height, Height);
            heightmap[Resolution, Resolution] = Random.Range(-Height, Height);
        }

        private void DiamondSquareAlgorithm(int row, int col, int size, float offset, float[,] heightmap)
        {
            int halfSize = (int)(size*0.5f);
            Vector2Int topLeft = new Vector2Int(col, row);
            Vector2Int topRight = new Vector2Int(col+size, row);
            Vector2Int bottomLeft = new Vector2Int(col, row+size);
            Vector2Int bottomRight = new Vector2Int(col+size, row+size);
            Vector2Int mid = new Vector2Int(halfSize+col, halfSize+row);

            // diamond step
            heightmap[mid.x,mid.y] = (heightmap[topLeft.x,topLeft.y] + heightmap[topRight.x,topRight.y] + heightmap[bottomLeft.x,bottomLeft.y] + heightmap[bottomRight.x,bottomRight.y]) * 0.25f + Random.Range(-offset, offset);

            // square step
            heightmap[topLeft.x+halfSize, topLeft.y] = (heightmap[topLeft.x, topLeft.y]+heightmap[topRight.x, topRight.y]+heightmap[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
            heightmap[mid.x-halfSize, mid.y] = (heightmap[topLeft.x, topLeft.y]+heightmap[bottomLeft.x, bottomLeft.y]+heightmap[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
            heightmap[mid.x+halfSize, mid.y] = (heightmap[topRight.x, topRight.y]+heightmap[bottomRight.x, bottomRight.y]+heightmap[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
            heightmap[bottomLeft.x+halfSize, bottomLeft.y] = (heightmap[bottomLeft.x, bottomLeft.y]+heightmap[bottomRight.x, bottomRight.y]+heightmap[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
        }
    }
}