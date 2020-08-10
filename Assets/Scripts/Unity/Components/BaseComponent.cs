using UnityEngine;

namespace Unity.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        protected TerrainData terrainData;

        void Start()
        {
            terrainData = Terrain.activeTerrain.terrainData;
        }

        public abstract void UpdateComponent();

        protected float[,] GetTerrainHeight()
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        }

        protected void UpdateTerrainHeight(float[,] heightmap)
        {
            terrainData.SetHeights(0, 0, heightmap);
        }
    }
}