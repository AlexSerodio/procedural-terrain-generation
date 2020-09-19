using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using Generation.Terrain.Physics.Erosion;
using TerrainGeneration.Analytics;
using Generation.Terrain.Physics.Erosion.GPU;
using Generation.Terrain.Procedural;
using Generation.Terrain.Procedural.GPU;

public class TestManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public Text SizeField;
    public Text SeedField;
    public Text ErosionFactorField;
    public Text ErosionTalusField;
    public Text ErosionIterationsField;

    public ComputeShader thermalErosionShader;
    public ComputeShader diamondSquareShader;

    // private MeshGenerator meshGenerator;
    private ThermalErosionComponent thermalErosionComponent;
    private DiamondSquareComponent diamondSquareComponent;

    void Start()
    {
        // meshGenerator = FindObjectOfType<MeshGenerator>();
        thermalErosionComponent = FindObjectOfType<ThermalErosionComponent>();
        diamondSquareComponent = FindObjectOfType<DiamondSquareComponent>();

        MenuPanel.SetActive(true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            MenuPanel.SetActive(!MenuPanel.activeSelf);
    }

    public void DiamondSquareButton()
    {
        diamondSquareComponent.seed = SeedField.text;
        // diamondSquareComponent.meshGenerator.resolution = int.Parse(SizeField.text);
        diamondSquareComponent.useGPU = true;

        diamondSquareComponent.UpdateComponent();
    }

    // public void DiamondSquareButton()
    // {
    //     string seed = SeedField.text;
    //     // int resolution = int.Parse(SizeField.text);
    //     bool useGPU = true;

    //     float[,] heightmap = meshGenerator.Heightmap;
    //     int resolution = meshGenerator.resolution;
    //     int intSeed = CovertStringSeedToInt(seed);

    //     DiamondSquare diamondSquare;
    //     if (useGPU)
    //         diamondSquare = new DiamondSquareGPU(resolution, diamondSquareShader, intSeed);
    //     else
    //         diamondSquare = new DiamondSquare(resolution, intSeed);

    //     TimeLogger.Start(diamondSquare.GetType().Name, diamondSquare.Resolution);
    //     diamondSquare.Apply(heightmap);
    //     TimeLogger.RecordSingleTimeInMilliseconds();

    //     meshGenerator.UpdateMesh(heightmap);
    // }

    public void ThermalErosionButton()
    {
        thermalErosionComponent.factor = float.Parse(ErosionFactorField.text);
        thermalErosionComponent.talusFactor = int.Parse(ErosionTalusField.text);
        thermalErosionComponent.iterations = int.Parse(ErosionIterationsField.text);
        thermalErosionComponent.useGPU = true;

        thermalErosionComponent.UpdateComponent();
    }

    // public void ThermalErosionButton()
    // {
    //     float factor = float.Parse(ErosionFactorField.text);
    //     int talusFactor = int.Parse(ErosionTalusField.text);
    //     int iterations = int.Parse(ErosionIterationsField.text);
    //     bool useGPU = true;

    //     float[,] heightmap = meshGenerator.Heightmap;
    //     int resolution = meshGenerator.resolution;

    //     int N = heightmap.GetLength(0);
    //     float talus = (float)talusFactor / N;

    //     ThermalErosion thermalErosion;
    //     if (useGPU)
    //         thermalErosion = new ThermalErosionGPU(thermalErosionShader);
    //     else
    //         thermalErosion = new ThermalErosion();

    //     TimeLogger.Start(thermalErosion.GetType().Name, resolution);
    //     thermalErosion.Erode(heightmap, talus, factor, iterations);
    //     TimeLogger.RecordSingleTimeInMilliseconds();

    //     meshGenerator.UpdateMesh(heightmap);
    // }

    // private int CovertStringSeedToInt(string stringSeed)
    // {
    //     if(string.IsNullOrWhiteSpace(stringSeed))
    //         return new System.Random().Next();

    //     var md5Hasher = System.Security.Cryptography.MD5.Create();
    //     var hashed = md5Hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringSeed));
    //     return System.BitConverter.ToInt32(hashed, 0);
    // }
}
