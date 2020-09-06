using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public int resolution;
        public float height;

        private DiamondSquare diamondSquare = new DiamondSquare();

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();

            diamondSquare.Resolution = base.meshGenerator.resolution;
            diamondSquare.Height = height;

            diamondSquare.Apply(heightmap);
            
            base.UpdateTerrainHeight(heightmap);
        }
    }
}
