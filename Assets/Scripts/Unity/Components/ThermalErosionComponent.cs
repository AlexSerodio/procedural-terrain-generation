using Generation.Terrain.Physics.Erosion;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class ThermalErosionComponent : BaseComponent
    {
        public float TalusFactor = 1;
        public float Factor = 0.5f;
        public int Iterations = 500;

        private ThermalErosion thermalErosion = new ThermalErosion();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            int N = heightmap.GetLength(0);
            float talus = TalusFactor / N;

            thermalErosion.Erode(heightmap, talus, Factor, Iterations);

            UpdateTerrainHeight(heightmap);
        }
    }
}
