using UnityEngine;

namespace Generation.Terrain.Procedural.GPU
{
    public class DiamondSquareGPU : ITerrainModifier
    {
        public int Resolution { get; set; }
        public float Height { get; set; }
        public ComputeShader Shader { get; set; }

        public DiamondSquareGPU()
        {
            Resolution = 1024;
            Height = 20;
        }

        public DiamondSquareGPU(ComputeShader shader) : this()
        {
            Shader = shader;
        }

        public DiamondSquareGPU(int resolution, float height, ComputeShader shader)
        {
            Resolution = resolution;
            Height = height;
            Shader = shader;
        }

        public void Apply(float[,] heightmap)
        {
            if(Shader == null)
                throw new System.InvalidOperationException("You must set at least the Shader property before call this operation.");

            RandomizeCorners(heightmap);

            int squareSize = Resolution;

            int i = 0;
            int numthreads = 0;
            while (squareSize > 1)
            {
                // Calculates the number of calls to DiamondSquare per iterations. Each call represents a new gpu thread
                numthreads = i > 0 ? (i * 2) : 1;
                i = numthreads;

                RunOnGPU(heightmap, squareSize, numthreads);

                squareSize /= 2;
                Height *= 0.5f;
            }
        }

        private void RandomizeCorners(float[,] heightmap)
        {
            heightmap[0, 0] = RandomValue() * Height;
            heightmap[0, Resolution] = RandomValue() * Height;
            heightmap[Resolution, 0] = RandomValue() * Height;
            heightmap[Resolution, Resolution] = RandomValue() * Height;
        }

        private void RunOnGPU(float[,] heightMap, float squareSize, int numthreads)
        {
            // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
            // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
            ComputeBuffer buffer = new ComputeBuffer(heightMap.Length, 4);

            // Set the initial data to be held in the buffer as the pre-generated heightmap
            buffer.SetData(heightMap);

            int kernelId = Shader.FindKernel("CSMain");     // Gets the id of the main kernel of the shader

            Shader.SetBuffer(kernelId, "terrain", buffer);  // Set the terrain buffer

            // Initializes the parameters needed for the shader
            Shader.SetInt("width", Resolution);
            Shader.SetFloat("height", Height);
            Shader.SetFloat("squareSize", squareSize);
            Shader.SetInt("externalSeed", new System.Random().Next());

            // Executes the compute shader on the GPU
            Shader.Dispatch(kernelId, numthreads, numthreads, 1);

            buffer.GetData(heightMap);          // Receive the updated heightmap data from the buffer
            buffer.Dispose();                   // Dispose the buffer 
        }

        private float RandomValue(float range = 1f)
        {
            return UnityEngine.Random.Range(-range, range);
        }
    }
}