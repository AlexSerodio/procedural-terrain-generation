using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Physics.Erosion.GPU;
using Generation.Terrain.Evaluation;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class ThermalErosionComponent : BaseComponent
    {
        public float factor;
        public float talusFactor;
        public int iterations;
        public ComputeShader shader;
        public bool useGPU;

        private ThermalErosion thermalErosion;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();
            int resolution = base.meshGenerator.resolution;

            int N = heightmap.GetLength(0);
            float talus = talusFactor / N;

            if (useGPU)
                thermalErosion = new ThermalErosionGPU(shader);
            else
                thermalErosion = new ThermalErosion();

            TimeLogger.Start(thermalErosion.GetType().Name, resolution);
            thermalErosion.Erode(heightmap, talus, factor, iterations);
            TimeLogger.RecordSingleTimeInMilliseconds();

            UpdateTerrainHeight(heightmap);

            float erosionScore = ErosionScore.Evaluate(heightmap);
            Debug.Log($"Erosion Score Thermal Erosion: {erosionScore}");
        }
    }
}
