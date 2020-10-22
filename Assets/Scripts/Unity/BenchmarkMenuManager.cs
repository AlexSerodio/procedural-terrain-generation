using TerrainGeneration.Analytics;
using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using System;

public class BenchmarkMenuManager : MonoBehaviour
{
    public GameObject BenchmarkMenu;
    public Text SeedField;
    public Text SizeField;
    public Text FactorField;
    public Text TalusField;
    public Text IterationsField;
    public Text RepetitionsField;
    public Dropdown AlgorithmDropdown;
    public Dropdown ArchitectureDropdown;

    private ThermalErosionComponent thermalErosionComponent;
    private DiamondSquareComponent diamondSquareComponent;
    private int repetitions;
    private string algorithm;
    private string architecture;

    private abstract class Algorithms
    {
        public const string DIAMOND_SQUARE = "Diamond-Square";
        public const string THERMAL_EROSION = "Thermal Erosion";
        public const string ALL = "All";
    }

    private abstract class Architecture
    {
        public const string CPU = "CPU";
        public const string GPU = "GPU";
        public const string ALL = "All";
    }

    void Start()
    {
        thermalErosionComponent = FindObjectOfType<ThermalErosionComponent>();
        diamondSquareComponent = FindObjectOfType<DiamondSquareComponent>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            BenchmarkMenu.SetActive(!BenchmarkMenu.activeSelf);
    }

    public void RunBenchmarkButton()
    {
        repetitions = int.Parse(RepetitionsField.text);
        algorithm = AlgorithmDropdown.options[AlgorithmDropdown.value].text;
        architecture = ArchitectureDropdown.options[ArchitectureDropdown.value].text;

        PrepareDiamondSquare();
        PrepareThermalErosion();

        switch (architecture)
        {
            case Architecture.CPU:
                RunAlgorithms(false);
                break;
            case Architecture.GPU:
                RunAlgorithms(true);
                break;
            case Architecture.ALL:
                RunAlgorithms(false);
                RunAlgorithms(true);
                break;
        }

        Debug.Log("Benchmark Completed!");
    }

    private void PrepareDiamondSquare()
    {
        diamondSquareComponent.seed = Convert.ToInt32(SeedField.text);
        diamondSquareComponent.meshGenerator.resolution = int.Parse(SizeField.text);
        diamondSquareComponent.meshGenerator.size = diamondSquareComponent.meshGenerator.resolution / 8;
        diamondSquareComponent.meshGenerator.heightFactor = diamondSquareComponent.meshGenerator.size / 6.0f;
    }

    private void PrepareThermalErosion()
    {
        thermalErosionComponent.factor = float.Parse(FactorField.text);
        thermalErosionComponent.talusFactor = float.Parse(TalusField.text);
        thermalErosionComponent.iterations = int.Parse(IterationsField.text);
    }

    private void RunAlgorithms(bool useGPU)
    {
        diamondSquareComponent.useGPU = useGPU;
        thermalErosionComponent.useGPU = useGPU;
        TimeLogger.Destination = "benchmark/";
        
        for (int i = 0; i < repetitions; i++)
        {
            PrepareDiamondSquare();

            diamondSquareComponent.UpdateComponent();
            thermalErosionComponent.UpdateComponent();
        }
    }
}
