using UnityEngine;

public class DiamondSquare : MonoBehaviour
{
    public int mDivisions;
    public float mSize;
    public float mHeight;

    private float[,] heights;

    void Start()
    {
        heights = new float[mDivisions+1, mDivisions+1];
        CreateTerrain();
    }

    private void CreateTerrain()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        RandomizeCorners();

        int iterations = (int)Mathf.Log(mDivisions, 2);
        int numSquares = 1;
        int squareSize = mDivisions;

        for (int i = 0; i < iterations; i++)
        {
            int row = 0;
            for (int j = 0; j < numSquares; j++)
            {
                int col = 0;
                for (int k = 0; k < numSquares; k++)
                {
                    DiamondSquareAlgorithm(row, col, squareSize, mHeight);
                    col += squareSize;
                }
                row += squareSize;
            }
            numSquares *= 2;
            squareSize /= 2;
            mHeight *= 0.5f;
        }
        mesh.vertices = GetVerticesFromMatrix();
        mesh.triangles = GetTriangles();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void RandomizeCorners()
    {
        heights[0, 0] = Random.Range(-mHeight, mHeight);
        heights[0, mDivisions] = Random.Range(-mHeight, mHeight);
        heights[mDivisions, 0] = Random.Range(-mHeight, mHeight);
        heights[mDivisions, mDivisions] = Random.Range(-mHeight, mHeight);
    }

    private void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfSize = (int)(size*0.5f);
        Vector2Int topLeft = new Vector2Int(col, row);
        Vector2Int topRight = new Vector2Int(col+size, row);
        Vector2Int bottomLeft = new Vector2Int(col, row+size);
        Vector2Int bottomRight = new Vector2Int(col+size, row+size);
        Vector2Int mid = new Vector2Int(halfSize+col, halfSize+row);

        // diamond step
        heights[mid.x,mid.y] = (heights[topLeft.x,topLeft.y] + heights[topRight.x,topRight.y] + heights[bottomLeft.x,bottomLeft.y] + heights[bottomRight.x,bottomRight.y]) * 0.25f + Random.Range(-offset, offset);

        // square step
        heights[topLeft.x+halfSize, topLeft.y] = (heights[topLeft.x, topLeft.y]+heights[topRight.x, topRight.y]+heights[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
        heights[mid.x-halfSize, mid.y] = (heights[topLeft.x, topLeft.y]+heights[bottomLeft.x, bottomLeft.y]+heights[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
        heights[mid.x+halfSize, mid.y] = (heights[topRight.x, topRight.y]+heights[bottomRight.x, bottomRight.y]+heights[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
        heights[bottomLeft.x+halfSize, bottomLeft.y] = (heights[bottomLeft.x, bottomLeft.y]+heights[bottomRight.x, bottomRight.y]+heights[mid.x, mid.y]) / 3 + Random.Range(-offset, offset);
    }

    private Vector3[] GetVerticesFromMatrix()
	{
		int xSize = heights.GetLength(0);
		int zSize = heights.GetLength(1);
		
		Vector3[] vectors = new Vector3[(xSize+1) * (zSize+1)];

        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

		float height;
		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
				if(x < xSize && z < zSize)
					height = heights[x, z];
				else if(x == xSize && z < zSize)
					height = heights[x-1, z];
				else if(x < xSize && z == zSize)
					height = heights[x, z-1];
				else
					height = heights[x-1, z-1];

				vectors[i++] = new Vector3(-halfSize + x * divisionSize, height, -halfSize + z * divisionSize);
			}
		}

		return vectors;
	}

    private int[] GetTriangles()
    {
		int xSize = heights.GetLength(0);
		int zSize = heights.GetLength(1);

		int trianglesAmount = xSize * zSize * 2 * 3;
        int[] triangles = new int[trianglesAmount];

        int vertexOffset = 0;
        int triangleOffset = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // since vertices are an array instead of a matrix, needs to add xSize to jump for next line
                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + xSize + 1;
                triangles[triangleOffset + 2] = vertexOffset + 1;

                triangles[triangleOffset + 3] = vertexOffset + 1;
                triangles[triangleOffset + 4] = vertexOffset + xSize + 1;
                triangles[triangleOffset + 5] = vertexOffset + xSize + 2;

                vertexOffset++;
                triangleOffset += 6;
            }
            vertexOffset++;
        }
		
		return triangles;
    }
}
