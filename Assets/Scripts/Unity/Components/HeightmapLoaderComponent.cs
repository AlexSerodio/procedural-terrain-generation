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

            heightmap = heightmap.Normalize();
            UpdateTerrainHeight(heightmap);
        }
    }
}
