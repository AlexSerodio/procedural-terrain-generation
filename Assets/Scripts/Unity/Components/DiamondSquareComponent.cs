using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public int resolution;
        public float height;
        public ComputeShader shader;
        public bool useGPU;

        private DiamondSquare diamondSquare = new DiamondSquare();
        private DiamondSquareGPU diamondSquareGPU = new DiamondSquareGPU();

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();

            if(!useGPU) {
                diamondSquare.Resolution = base.meshGenerator.resolution;
                diamondSquare.Height = height;

                TimeLogger.Start(LoggerType.CPU_DIAMOND_SQUARE);

                diamondSquare.Apply(heightmap);

                TimeLogger.RegisterTimeInMilliseconds();
                TimeLogger.SaveLog();
            }
            else
            {
                diamondSquareGPU.Resolution = base.meshGenerator.resolution;
                diamondSquareGPU.Height = height;
                diamondSquareGPU.Shader = shader;

                TimeLogger.Start(LoggerType.GPU_DIAMOND_SQUARE);

                diamondSquareGPU.Apply(heightmap);
            
                TimeLogger.RegisterTimeInMilliseconds();
                TimeLogger.SaveLog();
            }
            
            base.UpdateTerrainHeight(heightmap);
        }
    }
}
