using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public int resolution;
        public float height;
        public ComputeShader shader;
        public bool useGPU;

        private DiamondSquare diamondSquare = new DiamondSquare();
        private DiamondSquareGPU diamondSquareGPU = new DiamondSquareGPU();

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();

            if(!useGPU) {
                diamondSquare.Resolution = base.meshGenerator.resolution;
                diamondSquare.Height = height;
                diamondSquare.Apply(heightmap);
            }
            else
            {
                Debug.Log("Chamou GPU");
                // diamondSquareGPU.Resolution = base.meshGenerator.resolution;
                // diamondSquareGPU.Height = height;
                // diamondSquareGPU.Shader = shader;

                // diamondSquareGPU.Apply(heightmap);
            }

            
            base.UpdateTerrainHeight(heightmap);
        }
    }
}
