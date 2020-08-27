using UnityEngine;

public class DiamondSquareMatrix : MonoBehaviour
{
    public int mDivisions;
    public float mSize;
    public float mHeight;

    private Vector3[] mVerts;
    private int mVertCont;

    private float[,] heights;

    void Start()
    {
        heights = new float[mDivisions+1, mDivisions+1];
        CreateTerrain();

        Debug.Log("Vertex: " + mVerts.Length);
        Debug.Log("Heights: " + heights.GetLength(0)*heights.GetLength(1));
    }

    private void CreateTerrain()
    {
        mVertCont = (mDivisions + 1) * (mDivisions + 1);
        mVerts = new Vector3[mVertCont];
        Vector2[] uvs = new Vector2[mVertCont];
        int[] tris = new int[mDivisions * mDivisions * 2 * 3];    // 2 triangles per face and 3 vertex per triangle

        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;

        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f, -halfSize + i * divisionSize);  // just so the center of the mesh stays on the center of the world
                uvs[i * (mDivisions + 1) + j] = new Vector2((float)i / mDivisions, (float)j / mDivisions);

                if (i < mDivisions && j < mDivisions)
                {
                    int topLeft = i * (mDivisions + 1) + j;
                    int bottomLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = bottomLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = bottomLeft + 1;
                    tris[triOffset + 5] = bottomLeft;

                    triOffset += 6;
                }
            }
        }

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
        // mesh.vertices = mVerts;
        // mesh.triangles = tris;
        // mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void RandomizeCorners()
    {
        // TODO: remove this one
        mVerts[0].y = Random.Range(-mHeight, mHeight);
        mVerts[mDivisions].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight);

        heights[0, 0] = Random.Range(-mHeight, mHeight);
        heights[0, mDivisions] = Random.Range(-mHeight, mHeight);
        heights[mDivisions, 0] = Random.Range(-mHeight, mHeight);
        heights[mDivisions, mDivisions] = Random.Range(-mHeight, mHeight);
    }

    private void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfSize = (int)(size*0.5f);
        int topLeft = row * (mDivisions+1)+col;
        int bottomLeft = (row+size)*(mDivisions+1)+col;
        int mid = (int)(row+halfSize)*(mDivisions+1)+(int)(col+halfSize);

        Vector2Int topLeft2 = new Vector2Int(col, row);
        Vector2Int topRight2 = new Vector2Int(col+size, row);
        Vector2Int bottomLeft2 = new Vector2Int(col, row+size);
        Vector2Int bottomRight2 = new Vector2Int(col+size, row+size);
        Vector2Int mid2 = new Vector2Int(halfSize+col, halfSize+row);

        // diamond step
        mVerts[mid].y = (mVerts[topLeft].y+mVerts[topLeft+size].y+mVerts[bottomLeft].y+mVerts[bottomLeft+size].y) * 0.25f + Random.Range(-offset, offset);
        heights[mid2.x,mid2.y] = (heights[topLeft2.x,topLeft2.y] + heights[topRight2.x,topRight2.y] + heights[bottomLeft2.x,bottomLeft2.y] + heights[bottomRight2.x,bottomRight2.y]) * 0.25f + Random.Range(-offset, offset);
        // mVerts[mid].y = 20;
        // heights[halfSize+col,halfSize+row] = 20;

        // square step
        mVerts[topLeft+halfSize].y = (mVerts[topLeft].y+mVerts[topLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[mid-halfSize].y = (mVerts[topLeft].y+mVerts[bottomLeft].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[mid+halfSize].y = (mVerts[topLeft+size].y+mVerts[bottomLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[bottomLeft+halfSize].y = (mVerts[bottomLeft].y+mVerts[bottomLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);

        heights[topLeft2.x+halfSize, topLeft2.y] = (heights[topLeft2.x, topLeft2.y]+heights[topRight2.x, topRight2.y]+heights[mid2.x, mid2.y]) / 3 + Random.Range(-offset, offset);
        heights[mid2.x-halfSize, mid2.y] = (heights[topLeft2.x, topLeft2.y]+heights[bottomLeft2.x, bottomLeft2.y]+heights[mid2.x, mid2.y]) / 3 + Random.Range(-offset, offset);
        heights[mid2.x+halfSize, mid2.y] = (heights[topRight2.x, topRight2.y]+heights[bottomRight2.x, bottomRight2.y]+heights[mid2.x, mid2.y]) / 3 + Random.Range(-offset, offset);
        heights[bottomLeft2.x+halfSize, bottomLeft2.y] = (heights[bottomLeft2.x, bottomLeft2.y]+heights[bottomRight2.x, bottomRight2.y]+heights[mid2.x, mid2.y]) / 3 + Random.Range(-offset, offset);
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
                //  = new Vector3(-halfSize + j * divisionSize, 0.0f, halfSize - i * divisionSize);
			}
		}

		return vectors;
	}

    private int[] GetTriangles()
    {
		int xSize = heights.GetLength(0);
		int zSize = heights.GetLength(1);
        // int xSize = mDivisions;
		// int zSize = mDivisions;

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
