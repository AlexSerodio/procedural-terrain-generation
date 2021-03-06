﻿using System;
using Terrain.Utils;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Generation.Terrain.Procedural.GPU
{
    public class DiamondSquareGPU : DiamondSquare
    {
        public ComputeShader Shader { get; set; }

        private ComputeBuffer buffer;

        public DiamondSquareGPU(ComputeShader shader, int seed = 0)
            : base(seed) {
            Shader = shader;
        }

        public override void Apply(float[,] heightmap)
        {
            TimeLogger.Start(this.GetType().Name, heightmap.GetLength(0));
            
            base.Heightmap = heightmap;

            base.RandomizeCorners();

            int kernelId = InitComputeShader();

            float height = 1.0f;
            int squareSize = heightmap.GetLength(0)-1;
            int numthreads = 0;
            int totalThreads = 0;

            while (squareSize > 0)
            {
                // Calculates the number of calls to DiamondSquare per iterations. Each call represents a new gpu thread
                numthreads = numthreads > 0 ? (numthreads * 2) : 1;
                totalThreads += numthreads;

                RunComputeShader(kernelId, squareSize, height, numthreads);

                squareSize /= 2;
                height *= 0.5f;
            }

            FinishComputeShader();

            TimeLogger.RecordSingleTimeInMilliseconds();

            int size = Heightmap.GetLength(0) - 1;
            for (int x = 0; x <= size; x++) {
                Heightmap[x, size] = Heightmap[x, size-1] = Heightmap[x, size-2]  = Heightmap[x, size-3];
                Heightmap[x, 0] = Heightmap[x, 1];
            }

            Heightmap = Heightmap.Normalize();
        }

        private int InitComputeShader()
        {
            Array.Clear(Heightmap, 0, Heightmap.Length);
            
            // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
            // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
            buffer = new ComputeBuffer(Heightmap.Length, 4);

            // Set the initial data to be held in the buffer as the pre-generated heightmap
            buffer.SetData(Heightmap);

            int kernelId = Shader.FindKernel("CSMain");         // Gets the id of the main kernel of the shader

            Shader.SetBuffer(kernelId, "heightmap", buffer);    // Set the heightmap buffer
            Shader.SetInt("width", Heightmap.GetLength(0)-1);
            Shader.SetInt("externalSeed", RandomGenerator.Next());

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

        private void FinishComputeShader()
        {
            buffer.GetData(Heightmap);          // Receive the updated heightmap data from the buffer
            buffer.Dispose();                   // Dispose the buffer 
        }
    }
}
