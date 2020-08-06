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
        float talus = 1f / N;       // nosso talus é menor do que o do Olsen (4/N) possivelmente por estarmos trabalhando com diferenças de alturas maiores.
        float factor = 0.5f;
        int iterations = int.Parse(ErosionIterationsField.text);

        thermalErosion.Erode(heightMap, talus, factor, iterations);

        UpdateTerrain(heightMap);
    }

    public void LoadTerrain()
    {
        string fullPath = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\Heighmaps\\";
        string filename = LoadTerrainNameField.text;
        var reader = new ReadWriteTerrain(filename, fullPath);
        float[,] heightMap = reader.ReadMatrix();

        UpdateTerrain(heightMap);
    }

    public void SaveTerrain()
    {
        float[,] heightMap = GetHeightMap();

        string fullPath = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\Heighmaps\\";
        string filename = SaveTerrainNameField.text;
        var writer = new ReadWriteTerrain(filename, fullPath);

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
