using UnityEngine;

namespace Generation.Terrain.Physics.Erosion.GPU
{
    public class ThermalErosionGPU : ThermalErosion
    {
        public ComputeShader Shader { get; set; }

        private ComputeBuffer buffer;
        
        public ThermalErosionGPU(ComputeShader shader) : base()
        {
            Shader = shader;
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
            Shader.SetFloat("width", heightmap.GetLength(0)-1);

            return kernelId;
        }

        public override void Erode(float[,] heightmap, float talus = 4, float factor = 0.5f, int iterations = 500)
        {
            int kernelId = InitComputeShader(heightmap, talus, factor);

            for (int i = 0; i < iterations; i++)
                RunComputeShader(kernelId);
        
            FinishComputeShader(heightmap);
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
