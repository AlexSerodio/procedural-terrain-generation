using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Physics.Erosion.GPU;
using Terrain;
using Terrain.Evaluation;
using Terrain.Generation.Configurations;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class ThermalErosionComponent : BaseComponent
    {
        [Tooltip(TooltipHints.THERMAL_STRENGTH)]
        public float factor;
        [Tooltip(TooltipHints.THERMAL_TALUS)]
        public float talusFactor;
        [Tooltip(TooltipHints.ITERATIONS)]
        public int iterations;
        public ComputeShader shader;
        [Tooltip(TooltipHints.GPU_FLAG)]
        public bool useGPU;

        private ITerrainModifier terrainModifier;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();
            int resolution = base.meshGenerator.resolution;

            int N = heightmap.GetLength(0);
            float talus = talusFactor / N;

            ThermalErosionConfig configuration = new ThermalErosionConfig {
                Talus = talus,
                Strength = factor,
                Iterations = iterations
            };

            if (useGPU)
                terrainModifier = new ThermalErosionGPU(configuration, shader);
            else
                terrainModifier = new ThermalErosion(configuration);

            // TimeLogger.Start(terrainModifier.GetType().Name, resolution);
            terrainModifier.Apply(heightmap);
            // TimeLogger.RecordSingleTimeInMilliseconds();

            int seed = FindObjectOfType<DiamondSquareComponent>().seed;
            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);
            
            Debug.Log($"Thermal Erosion: {erosionScore} -> {benfordsLaw}");
            // EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), seed, erosionScore);
            // EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), seed, benfordsLaw);

            UpdateTerrainHeight(heightmap);
        }
    }
}
