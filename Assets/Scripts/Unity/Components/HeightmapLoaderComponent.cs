using Terrain.Evaluation;
using Terrain.Utils;
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

            loader.Load(texture, heightmap);

            Debug.Log($"Loaded Terrain: {ErosionScore.Evaluate(heightmap)} -> {BenfordsLaw.Evaluate(heightmap)}");
            
            UpdateTerrainHeight(heightmap);
        }
    }
}
