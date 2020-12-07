using Terrain.Evaluation;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Physics.Erosion.GPU;
using UnityEngine;
using Terrain.Generation.Configurations;
using Terrain;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class HydraulicErosionComponent : BaseComponent
    {
        [Tooltip(TooltipHints.HYDRAULIC_RAIN)]
        public float rainFactor;
        [Tooltip(TooltipHints.HYDRAULIC_SOLUBILITY)]
        public float solubility;
        [Tooltip(TooltipHints.HYDRAULIC_EVAPORATION)]
        public float evaporationFactor;
        [Tooltip(TooltipHints.ITERATIONS)]
        public int iterations;

        public ComputeShader pourAndDissolveShader;
        public ComputeShader waterFlowShader;
        public ComputeShader drainWaterShader;
        [Tooltip(TooltipHints.GPU_FLAG)]
        public bool useGPU;

        private ITerrainModifier terrainModifier;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            HydraulicErosionConfig configuration = new HydraulicErosionConfig {
                Rain = rainFactor,
                Solubility = solubility,
                Evaporation = evaporationFactor,
                Iterations = iterations
            };

            if(useGPU)
                terrainModifier = new HydraulicErosionGPU(configuration, pourAndDissolveShader, waterFlowShader, drainWaterShader);
            else
                terrainModifier = new HydraulicErosionDiegoliOptimized(configuration);

            // TimeLogger.Start(terrainModifier.GetType().Name, heightmap.GetLength(0));
            terrainModifier.Apply(heightmap);
            // TimeLogger.RecordSingleTimeInMilliseconds();

            int seed = FindObjectOfType<DiamondSquareComponent>().seed;
            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);

            Debug.Log($"Hydraulic Erosion: {erosionScore} -> {benfordsLaw}");
            // EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), seed, erosionScore);
            // EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), seed, benfordsLaw);

            UpdateTerrainHeight(heightmap);
        }
    }
}
