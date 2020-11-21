using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using Terrain.Evaluation;
using UnityEngine;
using Terrain;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public bool randomGeneration;

        // [ShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(randomGeneration))]
        public int seed;
        
        public ComputeShader shader;
        public bool useGPU;
        public bool onlyBestResults;

        // [DontShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(onlyBestResults))]
        public int tries;
        // [DontShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(onlyBestResults))]
        public int minimumFirstValue;

        private ITerrainModifier terrainModifier;

        public override void UpdateComponent()
        {
            if(randomGeneration)
                seed = new System.Random().Next();

            float[,] heightmap = base.GetTerrainHeight();

            if (useGPU)
                terrainModifier = new DiamondSquareGPU(shader, seed);
            else
                terrainModifier = new DiamondSquare(seed);

            // TimeLogger.Start(terrainModifier.GetType().Name, base.meshGenerator.resolution);

            if(!onlyBestResults)
                terrainModifier.Apply(heightmap);
            else 
            {
                terrainModifier.Apply(heightmap);
                int count = 0;
                while (!BenfordsLaw.StartsWith(heightmap, minimumFirstValue))
                {
                    if(count >= tries)
                        break;

                    terrainModifier.Apply(heightmap);
                    count++;
                }
            }
            // TimeLogger.RecordSingleTimeInMilliseconds();

            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);

            Debug.Log($"Diamond-Square: {erosionScore} -> {benfordsLaw}");
            // EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), seed, erosionScore);
            // EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), seed, benfordsLaw);

            base.UpdateTerrainHeight(heightmap);
        }
    }
}
