using Generation.Terrain.Evaluation;
using Generation.Terrain.Physics.Erosion;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class HydraulicErosionComponent : BaseComponent
    {
        public float rainFactor;
        public float solubility;
        public float evaporationFactor;
        public float sedimentCapacity;
        public int iterations;

        public bool diegoli;

        private HydraulicErosionDiegoli hydraulicErosionDiegoli;
        private HydraulicErosion hydraulicErosion;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            if(diegoli)
            {
                hydraulicErosionDiegoli = new HydraulicErosionDiegoli();
                hydraulicErosionDiegoli.Erode(heightmap, rainFactor, solubility, evaporationFactor, iterations);
            }
            else
            {
                hydraulicErosion = new HydraulicErosion();
                hydraulicErosion.Erode(heightmap, rainFactor, solubility, evaporationFactor, sedimentCapacity, iterations);
            }

            UpdateTerrainHeight(heightmap);

            float erosionScore = ErosionScore.Evaluate(heightmap);
            Debug.Log($"Erosion Score Hydraulic Erosion: {erosionScore}");
        }
    }
}
