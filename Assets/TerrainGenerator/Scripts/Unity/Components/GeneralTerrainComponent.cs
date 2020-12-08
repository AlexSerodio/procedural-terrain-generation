using Generation.Terrain.Physics.Erosion.GPU;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Procedural;
using Terrain.Utils;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class GeneralTerrainComponent : BaseComponent
    {
        public int smoothAmount;

        public ComputeShader shader;
        private DiamondSquare diamondSquare;
        ThermalErosion thermalErosion;
        ThermalErosionGPU thermalErosionGPU;

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
