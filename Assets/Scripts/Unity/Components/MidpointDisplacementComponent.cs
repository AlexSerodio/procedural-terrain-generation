using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class MidpointDisplacementComponent : BaseComponent
    {
        public float HeightDampenerPower = 2.0f;
        public float HeightMin = -2f;
        public float HeightMax = 2f;
        public float Roughness = 2.0f;

        private MidpointDisplacement MidpointDisplacement = new MidpointDisplacement();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();
        
            MidpointDisplacement.HeightDampenerPower = HeightDampenerPower;
            MidpointDisplacement.HeightMax = HeightMax;
            MidpointDisplacement.HeightMin = HeightMin;
            MidpointDisplacement.Roughness = Roughness;

            MidpointDisplacement.Apply(heightmap);
            
            UpdateTerrainHeight(heightmap);
        }
    }
}
