using Generation.Terrain.Evaluation;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Physics.Erosion.GPU;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class HydraulicErosionComponent : BaseComponent
    {
        public float rainFactor;
        public float solubility;
        public float evaporationFactor;
        public int iterations;

        public ComputeShader pourAndDissolveShader;
        public ComputeShader waterFlowShader;
        public ComputeShader drainWaterShader;
        public bool useGPU;

        private HydraulicErosionDiegoli hydraulicErosionDiegoli;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            if(useGPU)
                hydraulicErosionDiegoli = new HydraulicErosionGPU(pourAndDissolveShader, waterFlowShader, drainWaterShader);
            else
                hydraulicErosionDiegoli = new HydraulicErosionDiegoliOptimized();

            TimeLogger.Start(hydraulicErosionDiegoli.GetType().Name, heightmap.GetLength(0));
            hydraulicErosionDiegoli.Erode(heightmap, rainFactor, solubility, evaporationFactor, iterations);
            TimeLogger.RecordSingleTimeInMilliseconds();

            int seed = FindObjectOfType<DiamondSquareComponent>().seed;
            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);

            Debug.Log($"Hydraulic Erosion: {erosionScore} -> {benfordsLaw}");
            EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), seed, erosionScore);
            EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), seed, benfordsLaw);

            UpdateTerrainHeight(heightmap);
        }
    }
}
