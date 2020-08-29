using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
	public Gradient hypsometricMap;
    public float hypsometricMapFactor = 100f;
	public float heightFactor = 30f;

	private const int dimensions = 129;	
	
	private float[,] _heightmap;
	public float[,] Heightmap {
		get {
			if(_heightmap == null)
				_heightmap = new float[dimensions, dimensions];
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
		
		transform.localPosition = new Vector3(-dimensions/2, 0, -dimensions/2);
    }

    private Vector3[] GetVerticesFromMatrix()
	{
		int xSize = Heightmap.GetLength(0);
		int zSize = Heightmap.GetLength(1);
		
		Vector3[] vectors = new Vector3[(xSize+1) * (zSize+1)];

		float height;
		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
				if(x < xSize && z < zSize)
					height = Heightmap[x, z];
				else if(x == xSize && z < zSize)
					height = Heightmap[x-1, z];
				else if(x < xSize && z == zSize)
					height = Heightmap[x, z-1];
				else
					height = Heightmap[x-1, z-1];

				vectors[i++] = new Vector3(x, Math.Max(height * heightFactor, -50), z);
			}
		}

		return vectors;
	}

	private int[] GetTriangles()
    {
		int xSize = Heightmap.GetLength(0);
		int zSize = Heightmap.GetLength(1);

		int trianglesAmount = xSize * zSize * 6;
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
		Color[] colors = new Color[vertices.Length];
		for (int v = 0, y = 0; y < dimensions; y++) {
			for (int x = 0; x < dimensions; x++, v++) {
				colors[v] = hypsometricMap.Evaluate(vertices[v].y/hypsometricMapFactor);
			}
		}
		mesh.colors = colors;
	}
}
