using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Utils;

public class TestManager : MonoBehaviour
{
    public float ErosionFactor = 0.045f;
    public float ErosionStrength = 0.003f;
    private TerrainData terrainData;

    void Start()
    {
        terrainData = Terrain.activeTerrain.terrainData;
    }

    public void ThermalErosionMine()
    {
        float[,] heightMap = GetHeightMap();
        ThermalErosion thermalErosion = new ThermalErosion(ErosionStrength, ErosionFactor);
        thermalErosion.Erode(heightMap);

        UpdateTerrain(heightMap);
    }

    public void ThermalErosionDiegoli()
    {
        var thermalErosion = new Simulation.Terrain.DiegoliNeto.ThermalErosion();
        float[,] heightMap = GetHeightMap();

        int N = heightMap.GetLength(0);
        float talus = 2f / N;       // 0.0038f;
        float factor = 0.5f;
        int iteration = 500;
        for (int i = 0; i < iteration; i++)
            thermalErosion.DryErosion(heightMap, talus, factor);

        UpdateTerrain(heightMap);
    }

    public void RestartTerrain()
    {
        var reader = new ReadWriteTerrain("sample");
        float[,] heightMap = reader.ReadMatrix();

        UpdateTerrain(heightMap);
    }

    private float[,] GetHeightMap()
    {
        return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }

    private void UpdateTerrain(float[,] heightMap)
    {
        terrainData.SetHeights(0, 0, heightMap);
    }
}
