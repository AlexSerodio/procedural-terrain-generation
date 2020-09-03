using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class VoronoiComponent : BaseComponent
    {
        public float fallOff;
        public float dropOff;
        public float minHeight;
        public float maxHeight;
        public int peaksAmount;
        public VoronoiType type = VoronoiType.Combined;
        
        private Voronoi voronoi = new Voronoi();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            voronoi.DropOff = dropOff;
            voronoi.FallOff = fallOff;
            voronoi.MaxHeight = maxHeight;
            voronoi.MinHeight = minHeight;
            voronoi.PeaksAmount = peaksAmount;
            voronoi.Type = type;

            voronoi.Apply(heightmap);

            UpdateTerrainHeight(heightmap);
        }
    }
}
