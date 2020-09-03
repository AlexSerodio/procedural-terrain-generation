using Generation.Terrain.Physics.Erosion;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class ThermalErosionComponent : BaseComponent
    {
        public float factor;
        public float talusFactor;
        public int iterations;

        private ThermalErosion thermalErosion = new ThermalErosion();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            int N = heightmap.GetLength(0);
            float talus = talusFactor / N;

            thermalErosion.Erode(heightmap, talus, factor, iterations);

            UpdateTerrainHeight(heightmap);
        }
    }
}
