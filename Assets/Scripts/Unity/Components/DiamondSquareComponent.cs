using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using Generation.Terrain.Evaluation;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public bool randomGeneration;

        [ShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(randomGeneration))]
        public int seed;
        
        public ComputeShader shader;
        public bool useGPU;
        public bool onlyBestResults;

        [DontShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(onlyBestResults))]
        public int tries;
        [DontShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(onlyBestResults))]
        public int minimumFirstValue;

        private DiamondSquare diamondSquare;

        public override void UpdateComponent()
        {
            if(randomGeneration)
                seed = new System.Random().Next();

            float[,] heightmap = base.GetTerrainHeight();
            int resolution = base.meshGenerator.resolution;

            if (useGPU)
                diamondSquare = new DiamondSquareGPU(resolution, shader, seed);
            else
                diamondSquare = new DiamondSquare(resolution, seed);

            TimeLogger.Start(diamondSquare.GetType().Name, diamondSquare.Resolution);

            if(!onlyBestResults)
                diamondSquare.Apply(heightmap);
            else 
            {
                diamondSquare.Apply(heightmap);
                int count = 0;
                while (!BenfordsLaw.StartsWith(heightmap, minimumFirstValue))
                {
                    if(count >= tries)
                        break;

                    diamondSquare.Apply(heightmap);
                    count++;
                }
            }
            TimeLogger.RecordSingleTimeInMilliseconds();

            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);

            Debug.Log($"Diamond-Square: {erosionScore} -> {benfordsLaw}");
            // EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), seed, erosionScore);
            // EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), seed, benfordsLaw);

            base.UpdateTerrainHeight(heightmap);
        }
    }
}
