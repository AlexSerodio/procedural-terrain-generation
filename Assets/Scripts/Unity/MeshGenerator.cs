using Generation.Terrain.Procedural;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    private int xSize;
	private int zSize;
	private float heightFactor = 30f;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	private float[,] heightmap;

	void Start()
	{
		if (mesh == null) {
			mesh = new Mesh();
			mesh.name = "Mesh";
			GetComponent<MeshFilter>().mesh = mesh;
		}

		heightmap = new float[65, 65];

		var midpointDisplacement = new MidpointDisplacement();
		midpointDisplacement.HeightMin = -1f;
		midpointDisplacement.HeightMax = 1.5f;

		midpointDisplacement.Apply(heightmap);

		xSize = heightmap.GetLength(0);
		zSize = heightmap.GetLength(1);

		CreateShape(heightmap);
		UpdateMesh();
	}

	private void CreateShape(float[,] heightmap)
	{
        vertices = MatrixToVector3Array(heightmap);

        int trianglesAmount = xSize * zSize * 6;
        triangles = new int[trianglesAmount];

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
	}

	private Vector3[] MatrixToVector3Array(float[,] matrix)
	{
		Vector3[] vectors = new Vector3[(xSize+1) * (zSize+1)];

		float height;
		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
				if(x < xSize && z < zSize)
					height = matrix[x, z];
				else if(x == xSize && z < zSize)
					height = matrix[x-1, z];
				else if(x < xSize && z == zSize)
					height = matrix[x, z-1];
				else
					height = matrix[x-1, z-1];

				vectors[i++] = new Vector3(x, height * heightFactor, z);
			}
		}

		return vectors;
	}

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
	}

	private void OnDrawGizmos()
	{
		if(vertices == null)
			return;
		
		Gizmos.color = Color.red;
		for (int i = 0; i < vertices.Length; i++)
			Gizmos.DrawSphere(vertices[i], .1f);
	}
}
