using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class VoronoiComponent : BaseComponent
    {
        public float FallOff = 3.5f;
        public float DropOff = 2.5f;
        public float MinHeight = 0.14f;
        public float MaxHeight = 0.25f;
        public int PeaksAmount = 5;
        public VoronoiType Type = VoronoiType.Combined;
        
        private Voronoi voronoi = new Voronoi();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            voronoi.DropOff = DropOff;
            voronoi.FallOff = FallOff;
            voronoi.MaxHeight = MaxHeight;
            voronoi.MinHeight = MinHeight;
            voronoi.PeaksAmount = PeaksAmount;
            voronoi.Type = Type;

            voronoi.Apply(heightmap);

            UpdateTerrainHeight(heightmap);
        }
    }
}
