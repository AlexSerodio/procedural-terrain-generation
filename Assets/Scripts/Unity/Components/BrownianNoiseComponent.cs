using Generation.Terrain.Procedural.Noise;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class BrownianNoiseComponent : BaseComponent
    {
        public int PerlinOffsetX = 0;
        public int PerlinOffsetY = 0;
        public float PerlinXScale = 0.01f;
        public float PerlinYScale = 0.01f;
        public int PerlinOctaves = 3;
        public float PerlinPersistance = 8;
        public float PerlinHeightScale = 0.09f;
        
        private BrownianNoise brownianNoise = new BrownianNoise();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            brownianNoise.PerlinOffsetX = PerlinOffsetX;
            brownianNoise.PerlinOffsetY = PerlinOffsetY;
            brownianNoise.PerlinXScale = PerlinXScale;
            brownianNoise.PerlinYScale = PerlinYScale;
            brownianNoise.PerlinOctaves = PerlinOctaves;
            brownianNoise.PerlinPersistance = PerlinPersistance;
            brownianNoise.PerlinHeightScale = PerlinHeightScale;

            brownianNoise.Apply(heightmap);

            UpdateTerrainHeight(heightmap);
        }
    }
}
