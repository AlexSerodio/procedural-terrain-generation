using UnityEngine;

public class DiamondSquare : MonoBehaviour
{
    public int mDivisions;
    public float mSize;
    public float mHeight;

    private Vector3[] mVerts;
    private int mVertCont;

    void Start()
    {
        CreateTerrain();
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
                mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f, halfSize - i * divisionSize);  // just so the center of the mesh stays on the center of the world
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


        mesh.vertices = mVerts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void RandomizeCorners()
    {
        mVerts[0].y = Random.Range(-mHeight, mHeight);
        mVerts[mDivisions].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight);
    }

    private void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfSize = (int)(size*0.5f);
        int topLeft = row * (mDivisions+1)+col;
        int bottomLeft = (row+size)*(mDivisions+1)+col;
        int mid = (int)(row+halfSize)*(mDivisions+1)+(int)(col+halfSize);

        // diamond step
        mVerts[mid].y = (mVerts[topLeft].y+mVerts[topLeft+size].y+mVerts[bottomLeft].y+mVerts[bottomLeft+size].y) * 0.25f + Random.Range(-offset, offset);
    
        // square step
        mVerts[topLeft+halfSize].y = (mVerts[topLeft].y+mVerts[topLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[mid-halfSize].y = (mVerts[topLeft].y+mVerts[bottomLeft].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[mid+halfSize].y = (mVerts[topLeft+size].y+mVerts[bottomLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
        mVerts[bottomLeft+halfSize].y = (mVerts[bottomLeft].y+mVerts[bottomLeft+size].y+mVerts[mid].y) / 3 + Random.Range(-offset, offset);
    }
}
