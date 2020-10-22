using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using System;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public Text SizeField;
    public Text SeedField;
    public Toggle DiamondSquareGpu;
    public Text ErosionFactorField;
    public Text ErosionTalusField;
    public Text ErosionIterationsField;
    public Toggle ErosionGpu;

    private ThermalErosionComponent thermalErosionComponent;
    private DiamondSquareComponent diamondSquareComponent;

    void Start()
    {
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
        diamondSquareComponent.seed = Convert.ToInt32(SeedField.text);
        diamondSquareComponent.meshGenerator.resolution = int.Parse(SizeField.text);
        diamondSquareComponent.meshGenerator.size = diamondSquareComponent.meshGenerator.resolution / 8;
        diamondSquareComponent.meshGenerator.heightFactor = diamondSquareComponent.meshGenerator.size / 6.0f;
        diamondSquareComponent.useGPU = DiamondSquareGpu.isOn;

        diamondSquareComponent.UpdateComponent();
    }

    public void ThermalErosionButton()
    {
        thermalErosionComponent.factor = float.Parse(ErosionFactorField.text);
        thermalErosionComponent.talusFactor = float.Parse(ErosionTalusField.text);
        thermalErosionComponent.iterations = int.Parse(ErosionIterationsField.text);
        thermalErosionComponent.useGPU = ErosionGpu.isOn;

        thermalErosionComponent.UpdateComponent();
    }
}
