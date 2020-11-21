using Terrain.Generation.Configurations;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Generation.Terrain.Physics.Erosion.GPU
{
    public class ThermalErosionGPU : ThermalErosion
    {
        public ComputeShader Shader { get; }

        private ComputeBuffer buffer;
        
        public ThermalErosionGPU(ThermalErosionConfig configuration, ComputeShader shader)
            : base(configuration)
        {
            Shader = shader;
        }

        public override void Apply(float[,] heightmap)
        {
            TimeLogger.Start(this.GetType().Name, heightmap.GetLength(0));

            int kernelId = InitComputeShader(heightmap, Config.Talus, Config.Strength);

            for (int i = 0; i < Config.Iterations; i++)
                RunComputeShader(kernelId);
        
            FinishComputeShader(heightmap);

            TimeLogger.RecordSingleTimeInMilliseconds();
        }

        private int InitComputeShader(float[,] heightmap, float talus, float factor)
        {
            // Creates a read/writable buffer that contains the heightmap data and sends it to the GPU.
            // The buffer needs to be the same length as the heightmap, and each element in the heightmap is a single float which is 4 bytes long.
            buffer = new ComputeBuffer(heightmap.Length, 4);

            // Set the initial data to be held in the buffer as the pre-generated heightmap
            buffer.SetData(heightmap);

            int kernelId = Shader.FindKernel("CSMain");         // Gets the id of the main kernel of the shader

            Shader.SetBuffer(kernelId, "heightmap", buffer);    // Set the heightmap buffer
            Shader.SetFloat("talus", talus);
            Shader.SetFloat("factor", factor);
            Shader.SetInt("width", heightmap.GetLength(0));

            return kernelId;
        }

        private void RunComputeShader(int kernelId)
        {
            Shader.Dispatch(kernelId, 32, 32, 1);
        }

        private void FinishComputeShader(float[,] heightmap)
        {
            buffer.GetData(heightmap);          // Receive the updated heightmap data from the buffer
            buffer.Dispose();                   // Dispose the buffer 
        }
    }
}
