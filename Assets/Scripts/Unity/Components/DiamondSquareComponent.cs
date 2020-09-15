using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public ComputeShader shader;
        public bool useGPU;

        private DiamondSquare diamondSquare;

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();
            int resolution = base.meshGenerator.resolution;

            if (useGPU)
                diamondSquare = new DiamondSquareGPU(resolution, shader);
            else
                diamondSquare = new DiamondSquare(resolution);

            TimeLogger.Start(diamondSquare.GetType().Name, diamondSquare.Resolution);

            diamondSquare.Apply(heightmap);

            TimeLogger.RecordSingleTimeInMilliseconds();

            base.UpdateTerrainHeight(heightmap);
        }
    }
}
