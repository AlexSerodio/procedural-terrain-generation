using UnityEngine.UI;
using UnityEngine;
using Unity.Components;
using System;
using System.IO;
using System.Collections;
using SimpleFileBrowser;

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
        public Dropdown TerrainTextureDropdown;

        public Material[] terrainTextures;

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

            TerrainTextureDropdown.onValueChanged.AddListener(value => {
                FindObjectOfType<MeshGenerator>().GetComponent<Renderer>().material = terrainTextures[value];
            });

            DiamondSquareButton();
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
            {
                diamondSquareComponent.seed = Convert.ToInt32(SeedField.text);
                diamondSquareComponent.randomGeneration = false;
            }
            
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

        // TODO: block all screen interaction while dialog is open 
        public void ImportHeightmapButton()
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        public void ExportHeightmapButton()
        {
            ShowSaveDialogCoroutine();
        }

        public void QuitButton()
        {
            Application.Quit();
        }

        private IEnumerator ShowLoadDialogCoroutine()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));
            FileBrowser.SetDefaultFilter( ".jpg" );

            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Carregar imagem", "Carregar");

            if(FileBrowser.Success)
            {
                heightmapLoaderComponent.texture = LoadImageFromDisk(FileBrowser.Result[0]);
                heightmapLoaderComponent.UpdateComponent();
            }
        }

        private void ShowSaveDialogCoroutine()
        {
            float[,] heightmap = heightmapLoaderComponent.meshGenerator.Heightmap;
            FileBrowser.ShowSaveDialog((path) => SaveImageToDisk(path[0], heightmap), null, FileBrowser.PickMode.Files, false, "C:\\", "heightmap.png", "Salvar como", "Salvar");
        }

        private void SaveImageToDisk(string filePath, float[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    texture.SetPixel(y, x, new Color(image[x,y], image[x,y], image[x,y]));

            texture.Apply();

            File.WriteAllBytes(filePath, texture.EncodeToPNG());
        }

        private Texture2D LoadImageFromDisk(string filePath)
        {
            Texture2D texture = null;
            
            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);

                int size = heightmapLoaderComponent.meshGenerator.resolution;
                texture = new Texture2D(size, size);
                texture.LoadImage(fileData);
            }

            return texture;
        }
    }
}
