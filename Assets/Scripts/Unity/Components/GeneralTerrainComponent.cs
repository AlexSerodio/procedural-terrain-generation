using Generation.Terrain.Utils;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class GeneralTerrainComponent : BaseComponent
    {
        public int smoothAmount = 2;

        public void SmoothTerrain(int iterations)
        {
            float[,] heightmap = GetTerrainHeight();
            
            Smooth.Apply(heightmap, iterations);
            
            UpdateTerrainHeight(heightmap);
        }

        public void ResetTerrain()
        {
            float[,] heightmap = new float[xSize, zSize];
            
            UpdateTerrainHeight(heightmap);
        }

        public override void UpdateComponent()
        {
            throw new System.NotImplementedException();
        }
    }
}
