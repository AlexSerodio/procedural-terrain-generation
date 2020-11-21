using Terrain.Evaluation;
using Terrain.Utils;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class TerrainLoaderComponent : MonoBehaviour
    {
        public Texture2D texture;
        public UnityEngine.Terrain terrain;

        private HeightmapLoader loader = new HeightmapLoader();

        public void UpdateComponent()
        {
            int resolution = terrain.terrainData.heightmapResolution;
            float[,] heightmap = terrain.terrainData.GetHeights(0, 0, resolution, resolution);

            loader.Load(texture, heightmap);

            Debug.Log($"Erosion Score: {ErosionScore.Evaluate(heightmap)}");
            Debug.Log($"Benford's Law: {BenfordsLaw.Evaluate(heightmap)}");
            
            terrain.terrainData.SetHeights(0, 0, heightmap);
        }
    }
}
