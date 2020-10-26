using UnityEngine;

namespace Generation.Terrain.Physics.Erosion.GPU
{
    public class HydraulicErosionGPU : HydraulicErosionDiegoli
    {
        public ComputeShader PourAndDissolveShader { get; set; }
        public ComputeShader WaterFlowShader { get; set; }
        public ComputeShader DrainWaterShader { get; set; }

        private ComputeBuffer heightmapBuffer;
        private ComputeBuffer watermapBuffer;

        private const int threadGroupsX = 41;
        private const int threadGroupsY = 41;
        
        public HydraulicErosionGPU(ComputeShader pourAndDissolveShader, ComputeShader waterFlowShader, ComputeShader drainWaterShader) : base()
        {
            PourAndDissolveShader = pourAndDissolveShader;
            WaterFlowShader = waterFlowShader;
            DrainWaterShader = drainWaterShader;
        }

        public override void Erode(float[,] heightmap, float rainFactor, float solubility, float evaporationFactor, int iterations)
        {
            float[,] watermap = heightmap.Clone() as float[,];
            int width = heightmap.GetLength(0);

            heightmapBuffer = new ComputeBuffer(heightmap.Length, 4);
            watermapBuffer = new ComputeBuffer(watermap.Length, 4);
            
            heightmapBuffer.SetData(heightmap);
            watermapBuffer.SetData(watermap);

            int pourAndDissolveId = InitPourAndDissolveShader(rainFactor, solubility, width);
            int waterFlowId = InitWaterFlowShader(width);
            int drainWaterId = InitDrainWaterShader(evaporationFactor, solubility, width);

            for (int i = 0; i < iterations; i++)
                RunComputeShaders(pourAndDissolveId, waterFlowId, drainWaterId);
        
            FinishComputeShaders(heightmap, watermap);
        }

        private int InitPourAndDissolveShader(float rainFactor, float solubility, int width)
        {
            int kernelId = PourAndDissolveShader.FindKernel("CSMain");
            
            PourAndDissolveShader.SetBuffer(kernelId, "heightmap", heightmapBuffer);
            PourAndDissolveShader.SetBuffer(kernelId, "watermap", watermapBuffer);
            PourAndDissolveShader.SetFloat("rainFactor", rainFactor);
            PourAndDissolveShader.SetFloat("solubility", solubility);
            PourAndDissolveShader.SetInt("width", width);

            return kernelId;
        }

        private int InitWaterFlowShader(int width)
        {
            int kernelId = WaterFlowShader.FindKernel("CSMain");
            
            WaterFlowShader.SetBuffer(kernelId, "heightmap", heightmapBuffer);
            WaterFlowShader.SetBuffer(kernelId, "watermap", watermapBuffer);
            WaterFlowShader.SetInt("width", width);

            return kernelId;
        }

        private int InitDrainWaterShader(float evaporationFactor, float solubility, int width)
        {
            int kernelId = DrainWaterShader.FindKernel("CSMain");
            
            DrainWaterShader.SetBuffer(kernelId, "heightmap", heightmapBuffer);
            DrainWaterShader.SetBuffer(kernelId, "watermap", watermapBuffer);
            DrainWaterShader.SetFloat("evaporationFactor", evaporationFactor);
            DrainWaterShader.SetFloat("solubility", solubility);
            DrainWaterShader.SetInt("width", width);

            return kernelId;
        }

        private void RunComputeShaders(int pourAndDissolveId, int waterFlowId, int drainWaterId)
        {
            PourAndDissolveShader.Dispatch(pourAndDissolveId, threadGroupsX, threadGroupsY, 1);
            WaterFlowShader.Dispatch(waterFlowId, threadGroupsX, threadGroupsY, 1);
            DrainWaterShader.Dispatch(drainWaterId, threadGroupsX, threadGroupsY, 1);
        }

        private void FinishComputeShaders(float[,] heightmap, float[,] watermap)
        {
            heightmapBuffer.GetData(heightmap);
            heightmapBuffer.Dispose();
            
            watermapBuffer.GetData(watermap);
            watermapBuffer.Dispose();
        }
    }
}
