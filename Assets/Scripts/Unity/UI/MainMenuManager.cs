using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using System;
using System.IO;

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
        public Text TexturePathField;
        public Toggle HydraulicErosionGpu;

        private DiamondSquareComponent diamondSquareComponent;
        private ThermalErosionComponent thermalErosionComponent;
        private HydraulicErosionComponent hydraulicErosionComponent;

        private HeightmapLoaderComponent heightmapLoaderComponent;

        void Start()
        {
            diamondSquareComponent = FindObjectOfType<DiamondSquareComponent>();
            thermalErosionComponent = FindObjectOfType<ThermalErosionComponent>();
            hydraulicErosionComponent = FindObjectOfType<HydraulicErosionComponent>();
            heightmapLoaderComponent = FindObjectOfType<HeightmapLoaderComponent>();

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
            if(SeedField.text.Length == 0)
                diamondSquareComponent.randomGeneration = true;
            else
                diamondSquareComponent.randomGeneration = false;
            
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

        public void ImportHeightmapButton()
        {
            // TODO: recuperar de alguma forma
            string test = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\Assets\\Heightmaps\\1 Height Map (Merged).png";
            // heightmapLoaderComponent.texture = LoadImageFromDisk(TexturePathField.text);
            heightmapLoaderComponent.texture = LoadImageFromDisk(test);
            heightmapLoaderComponent.UpdateComponent();
        }

        private Texture2D LoadImageFromDisk(string filePath)
        {
            Texture2D texture = null;
            byte[] fileData;
        
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);

                int size = heightmapLoaderComponent.meshGenerator.resolution;
                texture = new Texture2D(size, size);
                texture.LoadImage(fileData);
            }

            return texture;
        }

        public void QuitButton()
        {
            Application.Quit();
        }
    }
}
