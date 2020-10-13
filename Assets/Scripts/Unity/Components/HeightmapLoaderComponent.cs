using Generation.Terrain.Evaluation;
using Generation.Terrain.Utils;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class HeightmapLoaderComponent : BaseComponent
    {
        public Texture2D texture;

        private HeightmapLoader loader = new HeightmapLoader();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            loader.Load(heightmap, texture);

            // heightmap = heightmap.Normalize();

            Debug.Log($"Erosion Score Diamond-Square: {ErosionScore.Evaluate(heightmap)}");
            Debug.Log($"Benford's Law Diamond-Square: {BenfordsLaw.Evaluate(heightmap)}");
            
            UpdateTerrainHeight(heightmap);
        }
    }
}
