using Generation.Terrain.Evaluation;
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

            Debug.Log($"Erosion Score: {ErosionScore.Evaluate(heightmap)}");
            Debug.Log($"Benford's Law: {BenfordsLaw.Evaluate(heightmap)}");
            
            UpdateTerrainHeight(heightmap);
        }
    }
}
