using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public int resolution;
    public float size;
    public float hypsometricMapFactor = 1;
    public Gradient hypsometricMap;

    private float[,] _heightmap;
    public float[,] Heightmap
    {
        get
        {
            if (_heightmap == null)
                _heightmap = new float[resolution + 1, resolution + 1];
            return _heightmap;
        }
        set => _heightmap = value;
    }

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Mesh";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void UpdateMesh(float[,] heightmap)
    {
        Heightmap = heightmap;

        vertices = GetVerticesFromMatrix();
        int[] triangles = GetTriangles();

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        UpdateHypsometricMap();

        mesh.RecalculateNormals();
    }

    private Vector3[] GetVerticesFromMatrix()
    {
        int xSize = Heightmap.GetLength(0);
        int zSize = Heightmap.GetLength(1);

        Vector3[] vectors = new Vector3[(xSize + 1) * (zSize + 1)];

        float halfSize = size * 0.5f;
        float divisionSize = size / resolution;

        float height;
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (x < xSize && z < zSize)
                    height = Heightmap[x, z];
                else if (x == xSize && z < zSize)
                    height = Heightmap[x - 1, z];
                else if (x < xSize && z == zSize)
                    height = Heightmap[x, z - 1];
                else
                    height = Heightmap[x - 1, z - 1];

                vectors[i++] = new Vector3(-halfSize + x * divisionSize, height, -halfSize + z * divisionSize);
            }
        }

        return vectors;
    }

    private int[] GetTriangles()
    {
        int xSize = Heightmap.GetLength(0);
        int zSize = Heightmap.GetLength(1);

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

    public void UpdateHypsometricMap()
    {
        int xSize = Heightmap.GetLength(0);
        int zSize = Heightmap.GetLength(1);
        Color[] colors = new Color[vertices.Length];

        for (int v = 0, y = 0; y < xSize; y++)
        {
            for (int x = 0; x < zSize; x++, v++)
            {
                colors[v] = hypsometricMap.Evaluate(vertices[v].y / hypsometricMapFactor);
            }
        }
        mesh.colors = colors;
    }
}
