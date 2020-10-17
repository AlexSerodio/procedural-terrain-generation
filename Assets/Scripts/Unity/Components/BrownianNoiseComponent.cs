using Generation.Terrain.Evaluation;
using Generation.Terrain.Procedural.Noise;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class BrownianNoiseComponent : BaseComponent
    {
        public int perlinOffsetX;
        public int perlinOffsetY;
        public float perlinXScale;
        public float perlinYScale;
        public int perlinOctaves;
        public float perlinPersistance;
        public float perlinHeightScale;

        private BrownianNoise brownianNoise = new BrownianNoise();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            brownianNoise.PerlinOffsetX = perlinOffsetX;
            brownianNoise.PerlinOffsetY = perlinOffsetY;
            brownianNoise.PerlinXScale = perlinXScale;
            brownianNoise.PerlinYScale = perlinYScale;
            brownianNoise.PerlinOctaves = perlinOctaves;
            brownianNoise.PerlinPersistance = perlinPersistance;
            brownianNoise.PerlinHeightScale = perlinHeightScale;

            brownianNoise.Apply(heightmap);

            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);
            Debug.Log($"Erosion Score Diamond-Square: {erosionScore}");
            Debug.Log($"Benford's Law Diamond-Square: {benfordsLaw}");
            // EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), erosionScore);
            // EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), benfordsLaw);

            UpdateTerrainHeight(heightmap);
        }
    }
}
