using Generation.Terrain.Procedural;
using Generation.Terrain.Utils;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    private int xSize;
	private int zSize;

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

		// string filename = Files.HeightmapPath + "heightmap-255-1";
        // heightmap = HeightmapSerializer.Deserialize(filename);

		heightmap = new float[65, 65];

		var midpointDisplacement = new MidpointDisplacement();

		midpointDisplacement.Apply(heightmap);

		xSize = heightmap.GetLength(0);
		zSize = heightmap.GetLength(1);

        // xSize = 255;
		// zSize = 255;

		// vertices = MatrixToVector3Array(heightmap);

		CreateShape(heightmap);
		UpdateMesh();
	}

	private void CreateShape(float[,] heightmap)
	{
		// vertices = new Vector3[(xSize + 1) * (zSize + 1)];

		// for (int i = 0, z = 0; z <= zSize; z++)
		// {
		// 	for (int x = 0; x <= xSize; x++)
		// 	{
		// 		vertices[i++] = new Vector3(x, 0, z);
		// 	}
		// }
        vertices = MatrixToVector3Array(heightmap);

        int trianglesAmount = (xSize) * (zSize) * 6;
        triangles = new int[trianglesAmount];

		Debug.Log("Triangles: " + triangles.Length / 3);

        int vertexOffset = 0;
        int triangleOffset = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize && triangleOffset < trianglesAmount; x++)
            {
				// Debug.Log(vertexOffset);
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

	private void UpdateMesh()
	{
		mesh.Clear();

		Debug.Log("Vertices: " + vertices.Length);
		Debug.Log("Triangles: " + triangles.Length / 3);

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		// string indices = "";
		// foreach (int index in triangles)
		// 	indices += index + ", ";
		// Debug.Log(indices);

		mesh.RecalculateNormals();
	}

	private Vector3[] MatrixToVector3Array(float[,] matrix)
	{
		// Debug.Log(xSize);
		// Debug.Log(zSize);
		matrix = ExtendMatrix(matrix);

		// xSize = matrix.GetLength(0);
		// zSize = matrix.GetLength(1);

		// Debug.Log(xSize);
		// Debug.Log(zSize);

		Vector3[] vectors = new Vector3[(xSize+1) * (zSize+1)];

		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
				vectors[i++] = new Vector3(x, matrix[x, z]*30f, z);
		}

		return vectors;
	}

	private float[,] ExtendMatrix(float[,] matrix)
	{
		int width = matrix.GetLength(0);
		int height = matrix.GetLength(1);

		float[,] extended = new float[width+1, height+1];

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				extended[i, j] = matrix[i,j];
				extended[i+1, j+1] = matrix[i,j];
			}
		}

		// for (int i = 0; i < width; i++)
		// 	extended[i, height] = extended[i, height-1];

		// for (int y = 0; y < height; y++)
		// 	extended[width, y] = extended[width-1, y];

		return extended;
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
