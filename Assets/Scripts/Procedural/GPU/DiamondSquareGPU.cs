using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondSquareGPU : MonoBehaviour
{
    public ComputeShader shader;

    private int resolution = 1024;
    private float size = 128;
    private float height = 20.0f;

    private float[,] heightmap;

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        heightmap = new float[resolution+1, resolution+1];

        mesh = new Mesh();
        mesh.name = "Mesh";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        GetComponent<MeshFilter>().mesh = mesh;

        Apply(heightmap);

        UpdateMesh();
    }

    public void Apply(float[,] heightmap)
    {
        RandomizeCorners(heightmap);

        int squareSize = resolution;

        int i = 0;
        int numthreads = 0;
        while (squareSize > 1)
        {
            // Calculates the number of calls to DiamondSquare per iterations. Each call represents a new gpu thread
            numthreads = i > 0 ? (i * 2) : 1;
            i = numthreads;

            RunOnGPU(heightmap, squareSize, numthreads);

            squareSize /= 2;
            height *= 0.5f;
        }
    }

    private void RandomizeCorners(float[,] heightmap)
    {
        heightmap[0, 0] = RandomValue() * height;
        heightmap[0, resolution] = RandomValue() * height;
        heightmap[resolution, 0] = RandomValue() * height;
        heightmap[resolution, resolution] = RandomValue() * height;
    }

    private float RandomValue(float range = 1)
    {
        return UnityEngine.Random.Range(-range, range);
    }

    private void RunOnGPU(float[,] heightMap, float squareSize, int numthreads)
    {
        // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
        // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
        ComputeBuffer buffer = new ComputeBuffer(heightMap.Length, 4);

        // Set the initial data to be held in the buffer as the pre-generated heightmap
        buffer.SetData(heightMap);

        int kernelId = shader.FindKernel("CSMain");     // Gets the id of the main kernel of the shader

        shader.SetBuffer(kernelId, "terrain", buffer);  // Set the terrain buffer

        // Initializes the parameters needed for the shader
        shader.SetInt("width", resolution);
        shader.SetFloat("height", height);
        shader.SetFloat("squareSize", squareSize);
        shader.SetInt("externalSeed", new System.Random().Next());

        // Executes the compute shader on the GPU
        shader.Dispatch(kernelId, numthreads, numthreads, 1);

        buffer.GetData(heightMap);          // Receive the updated heightmap data from the buffer
        buffer.Dispose();                   // Dispose the buffer 
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = GetVerticesFromMatrix();
        mesh.triangles = GetTriangles();
        mesh.RecalculateNormals();
    }

    private Vector3[] GetVerticesFromMatrix()
    {
        int xSize = heightmap.GetLength(0);
        int zSize = heightmap.GetLength(1);

        Vector3[] vectors = new Vector3[(xSize + 1) * (zSize + 1)];

        float halfSize = size * 0.5f;
        float divisionSize = size / resolution;

        float height;
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (x < xSize && z < zSize)
                    height = heightmap[x, z];
                else if (x == xSize && z < zSize)
                    height = heightmap[x-1, z];
                else if (x < xSize && z == zSize)
                    height = heightmap[x, z-1];
                else
                    height = heightmap[x-1, z-1];

                vectors[i++] = new Vector3(-halfSize + x * divisionSize, height, -halfSize + z * divisionSize);
            }
        }

        return vectors;
    }

    private int[] GetTriangles()
    {
        int xSize = heightmap.GetLength(0);
        int zSize = heightmap.GetLength(1);

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
