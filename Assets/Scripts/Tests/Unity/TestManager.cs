using Generation.Terrain.Utils;
using Simulation.Terrain.DiegoliNeto;
using UnityEngine.UI;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    private TerrainData terrainData;

    public Text LoadTerrainNameField;
    public Text SaveTerrainNameField;
    public Text ErosionIterationsField;
    public Text SmoothAmountField;

    void Start()
    {
        terrainData = Terrain.activeTerrain.terrainData;
    }

    public void ThermalErosion()
    {
        var thermalErosion = new ThermalErosion();
        float[,] heightMap = GetHeightMap();

        int N = heightMap.GetLength(0);
        float talus = 1f / N;       // our talus is smaller than Olsen's (4/N) possibly because we're working with greater height differences.
        float factor = 0.5f;
        int iterations = int.Parse(ErosionIterationsField.text);

        thermalErosion.Erode(heightMap, talus, factor, iterations);

        UpdateTerrain(heightMap);
    }

    public void LoadTerrain()
    {
        string filename = LoadTerrainNameField.text;
        var reader = new ReadWriteTerrain(filename, Files.HeightmapPath);
        float[,] heightMap = reader.ReadMatrix();

        UpdateTerrain(heightMap);
    }

    public void SaveTerrain()
    {
        float[,] heightMap = GetHeightMap();

        string filename = SaveTerrainNameField.text;
        var writer = new ReadWriteTerrain(filename, Files.HeightmapPath);

        writer.WriteMatrix(heightMap);
    }

    public void Smooth()
    {
        float[,] heightMap = GetHeightMap();

        int iterations = int.Parse(SmoothAmountField.text);
        TerrainUtils.Smooth(heightMap, iterations);

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
