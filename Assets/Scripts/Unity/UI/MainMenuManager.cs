using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using System;

namespace Unity.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject MenuPanel;
        public Text SeedField;
        public Toggle DiamondSquareGpu;
        
        public Text ThermalErosionFactorField;
        public Text ThermalErosionTalusField;
        public Text ThermalErosionIterationsField;
        public Toggle ThermalErosionGpu;

        public Text HydraulicErosionRainField;
        public Text HydraulicErosionSolubilityField;
        public Text HydraulicErosionEvaporationField;
        public Text HydraulicErosionIterationsField;
        public Toggle HydraulicErosionGpu;

        private DiamondSquareComponent diamondSquareComponent;
        private ThermalErosionComponent thermalErosionComponent;
        private HydraulicErosionComponent hydraulicErosionComponent;

        void Start()
        {
            diamondSquareComponent = FindObjectOfType<DiamondSquareComponent>();
            thermalErosionComponent = FindObjectOfType<ThermalErosionComponent>();
            hydraulicErosionComponent = FindObjectOfType<HydraulicErosionComponent>();

            MenuPanel.SetActive(true);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                MenuPanel.SetActive(!MenuPanel.activeSelf);
        }

        public void DiamondSquareButton()
        {
            if(SeedField.text.Length == 0)
                diamondSquareComponent.randomGeneration = true;
            else            
                diamondSquareComponent.seed = Convert.ToInt32(SeedField.text);
            
            // diamondSquareComponent.meshGenerator.resolution = 65;
            diamondSquareComponent.useGPU = DiamondSquareGpu.isOn;

            diamondSquareComponent.UpdateComponent();
        }

        public void ThermalErosionButton()
        {
            thermalErosionComponent.factor = float.Parse(ThermalErosionFactorField.text);
            thermalErosionComponent.talusFactor = float.Parse(ThermalErosionTalusField.text);
            thermalErosionComponent.iterations = int.Parse(ThermalErosionIterationsField.text);
            thermalErosionComponent.useGPU = ThermalErosionGpu.isOn;

            thermalErosionComponent.UpdateComponent();
        }

        public void HydraulicErosionButton()
        {
            hydraulicErosionComponent.rainFactor = float.Parse(HydraulicErosionRainField.text);
            hydraulicErosionComponent.solubility = float.Parse(HydraulicErosionSolubilityField.text);
            hydraulicErosionComponent.evaporationFactor = float.Parse(HydraulicErosionEvaporationField.text);
            hydraulicErosionComponent.iterations = int.Parse(HydraulicErosionIterationsField.text);
            hydraulicErosionComponent.useGPU = HydraulicErosionGpu.isOn;

            hydraulicErosionComponent.UpdateComponent();
        }

        public void QuitButton()
        {
            Application.Quit();
        }
    }
}
