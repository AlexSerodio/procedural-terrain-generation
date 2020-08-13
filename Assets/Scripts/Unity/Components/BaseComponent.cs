using UnityEngine;

namespace Unity.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        // private TerrainData terrainData;
        public MeshGenerator meshGenerator;
        protected int xSize { get => meshGenerator.Heightmap.GetLength(0); }
        protected int zSize { get => meshGenerator.Heightmap.GetLength(1); }

        void Start()
        {
            meshGenerator = FindObjectOfType<MeshGenerator>();
        }

        public abstract void UpdateComponent();

        protected float[,] GetTerrainHeight()
        {
            return meshGenerator.Heightmap;
        }

        protected void UpdateTerrainHeight(float[,] heightmap)
        {
            meshGenerator.UpdateMesh(heightmap);
        }

        // protected float[,] GetTerrainHeight()
        // {
        //     return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        // }

        // protected void UpdateTerrainHeight(float[,] heightmap)
        // {
        //     terrainData.SetHeights(0, 0, heightmap);
        // }
    }
}