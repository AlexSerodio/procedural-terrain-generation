﻿using Generation.Terrain.Utils;
using UnityEngine;

namespace Generation.Terrain.Procedural.GPU
{
    public class DiamondSquareGPU : DiamondSquare
    {
        public ComputeShader Shader { get; set; }

        private ComputeBuffer buffer;

        public DiamondSquareGPU(int resolution, ComputeShader shader, int seed = 0)
            : base(resolution, seed) {
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

            while (squareSize > 1)
            {
                // Calculates the number of calls to DiamondSquare per iterations. Each call represents a new gpu thread
                numthreads = i > 0 ? (i * 2) : 1;
                i = numthreads;

                RunComputeShader(kernelId, squareSize, height, numthreads);

                squareSize /= 2;
                height *= 0.5f;
            }

            FinishComputeShader(heightmap);

            heightmap = heightmap.Normalize();
        }

        private int InitComputeShader(float[,] heightmap)
        {
            // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
            // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
            buffer = new ComputeBuffer(heightmap.Length, 4);

            // Set the initial data to be held in the buffer as the pre-generated heightmap
            buffer.SetData(heightmap);

            int kernelId = Shader.FindKernel("CSMain");         // Gets the id of the main kernel of the shader

            Shader.SetBuffer(kernelId, "heightmap", buffer);    // Set the heightmap buffer
            Shader.SetInt("width", Resolution);
            Shader.SetInt("externalSeed", random.Next());

            return kernelId;
        }

        private void RunComputeShader(int kernelId, float squareSize, float height, int numthreads)
        {
            // Initializes the parameters needed for the shader
            Shader.SetFloat("height", height);
            Shader.SetFloat("squareSize", squareSize);

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
