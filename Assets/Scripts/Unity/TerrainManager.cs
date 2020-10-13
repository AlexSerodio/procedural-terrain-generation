using Generation.Terrain.Evaluation;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        int resolution = terrain.terrainData.heightmapResolution;
        float[,] heightmap = terrain.terrainData.GetHeights(0, 0, resolution, resolution);

        Debug.Log($"Erosion Score Diamond-Square: {ErosionScore.Evaluate(heightmap)}");
        Debug.Log($"Benford's Law Diamond-Square: {BenfordsLaw.Evaluate(heightmap)}");
    }
}
