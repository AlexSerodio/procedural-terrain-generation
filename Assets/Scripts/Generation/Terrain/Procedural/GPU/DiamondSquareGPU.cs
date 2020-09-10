using UnityEngine;

namespace Generation.Terrain.Procedural.GPU
{
    public class DiamondSquareGPU : DiamondSquare
    {
        public ComputeShader Shader { get; set; }

        private ComputeBuffer buffer;

        public DiamondSquareGPU(ComputeShader shader) : base()
        {
            Shader = shader;
        }

        public DiamondSquareGPU(int resolution, ComputeShader shader)
        {
            Resolution = resolution;
            Shader = shader;
        }

        public override void Apply(float[,] heightmap)
        {
            base.RandomizeCorners(heightmap);

            float height = 1.0f;
            int squareSize = Resolution;

            int i = 0;
            int numthreads = 0;

            int kernelId = InitComputeShader(heightmap);

            int totalThreadsUsed = 0;
            while (squareSize > 1)
            {
                // Calculates the number of calls to DiamondSquare per iterations. Each call represents a new gpu thread
                numthreads = i > 0 ? (i * 2) : 1;
                i = numthreads;

                RunOnGPU(kernelId, squareSize, height, numthreads);

                squareSize /= 2;
                height *= 0.5f;

                // For debug purposes only
                totalThreadsUsed += numthreads;
            }

            FinishComputeShader(heightmap);

            // For debug purposes only
            Debug.Log($"Number of threads used: {totalThreadsUsed}.");
        }

        private int InitComputeShader(float[,] heightmap)
        {
            // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
            // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
            buffer = new ComputeBuffer(heightmap.Length, 4);

            // Set the initial data to be held in the buffer as the pre-generated heightmap
            buffer.SetData(heightmap);

            int kernelId = Shader.FindKernel("CSMain");     // Gets the id of the main kernel of the shader

            Shader.SetBuffer(kernelId, "terrain", buffer);  // Set the terrain buffer
            Shader.SetInt("width", Resolution);

            return kernelId;
        }

        private void RunOnGPU(int kernelId, float squareSize, float height, int numthreads)
        {
            // Initializes the parameters needed for the shader
            Shader.SetFloat("height", height);
            Shader.SetFloat("squareSize", squareSize);
            Shader.SetInt("externalSeed", new System.Random().Next());

            // Executes the compute shader on the GPU
            Shader.Dispatch(kernelId, numthreads, numthreads, 1);
        }

        private void FinishComputeShader(float[,] heightmap)
        {
            buffer.GetData(heightmap);          // Receive the updated heightmap data from the buffer
            buffer.Dispose();                   // Dispose the buffer 
        }
    }
}
