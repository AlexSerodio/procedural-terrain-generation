using Generation.Terrain.Physics.Erosion.GPU;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using Terrain.Generation.Configurations;
using TerrainGeneration.Analytics;
using UnityEngine.UI;
using UnityEngine;

namespace Unity.UI
{
    public class BenchmarkMenuManager : MonoBehaviour
    {
        public GameObject BenchmarkMenu;

        public Text FactorField;
        public Text TalusField;

        public Text RainField;
        public Text SolubilityField;
        public Text EvaporationField;

        public Text IterationsField;

        public Dropdown AlgorithmDropdown;
        public Dropdown ArchitectureDropdown;

        public ComputeShader diamondShader;
        public ComputeShader thermalShader;
        public ComputeShader hydraulicRainShader;
        public ComputeShader hydraulicWaterflowShader;
        public ComputeShader hydraulicDrainWaterShader;

        private int repetitions;
        private string algorithm;
        private string architecture;

        private DiamondSquare diamondSquare;
        private ThermalErosion thermalErosion;
        private HydraulicErosionDiegoli hydraulicErosion;

        private DiamondSquareGPU diamondSquareGPU;
        private ThermalErosionGPU thermalErosionGPU;
        private HydraulicErosionGPU hydraulicErosionGPU;

        private ThermalErosionConfig thermalConfig;
        private HydraulicErosionConfig hydraulicConfig;

        private abstract class Algorithms
        {
            public const string DIAMOND_SQUARE = "Diamond-Square";
            public const string THERMAL_EROSION = "Thermal Erosion";
            public const string HYDRAULIC_EROSION = "Hydraulic Erosion";
            public const string ALL = "All";
        }

        private abstract class Architecture
        {
            public const string CPU = "CPU";
            public const string GPU = "GPU";
            public const string ALL = "All";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                BenchmarkMenu.SetActive(!BenchmarkMenu.activeSelf);
        }

        public void RunBenchmarkButton()
        {
            thermalConfig = new ThermalErosionConfig {
                Talus = float.Parse(TalusField.text),
                Strength = float.Parse(FactorField.text),
                Iterations = int.Parse(IterationsField.text)
            };

            hydraulicConfig = new HydraulicErosionConfig {
                Rain = float.Parse(RainField.text),
                Solubility = float.Parse(SolubilityField.text),
                Evaporation = float.Parse(EvaporationField.text),
                Iterations = int.Parse(IterationsField.text)
            };

            algorithm = AlgorithmDropdown.options[AlgorithmDropdown.value].text;
            architecture = ArchitectureDropdown.options[ArchitectureDropdown.value].text;

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

        private void RunAlgorithms(bool useGPU)
        {
            int resolution = 1025;
            string architecture = useGPU ? "GPU" : "CPU";

            foreach (int iteration in iterations)
            {
                thermalConfig.Iterations = iteration;
                hydraulicConfig.Iterations = iteration;

                float[,] heightmap = new float[resolution, resolution];

                TimeLogger.Destination = $"benchmark/{architecture}/{thermalConfig.Iterations}/";

                if(!useGPU)
                {
                    foreach (int seed in seeds)
                    {
                        diamondSquare = new DiamondSquare(seed);
                        thermalErosion = new ThermalErosion(thermalConfig);
                        hydraulicErosion = new HydraulicErosionDiegoli(hydraulicConfig);

                        diamondSquare.Apply(heightmap);
                        thermalErosion.Apply(heightmap);
                        hydraulicErosion.Apply(heightmap);
                    }
                }
                else
                {
                    foreach (int seed in seeds)
                    {
                        diamondSquareGPU = new DiamondSquareGPU(diamondShader, seed);
                        thermalErosionGPU = new ThermalErosionGPU(thermalConfig, thermalShader);
                        hydraulicErosionGPU = new HydraulicErosionGPU(hydraulicConfig, hydraulicRainShader, hydraulicWaterflowShader, hydraulicDrainWaterShader);

                        diamondSquareGPU.Apply(heightmap);
                        thermalErosionGPU.Apply(heightmap);
                        hydraulicErosionGPU.Apply(heightmap);
                    }
                }
            }
        }

        // private int[] resolutions = new int[] {
        //     65, 129, 257, 513, 1025
        // };

        private int[] iterations = new int[] {
            100, 200, 300, 400, 500
        };

        private int[] seeds = new int[] {
            1428799138,
            1428799138,
            2070800132,
            464531400,
            705741660,
            303002533,
            438907051,
            2003470750,
            949691303,
            928586617,
            877219381,
            1237917249,
            1060139585,
            1720505882,
            1130495001,
            76715554,
            870751325,
            273649511,
            2130790706,
            1464243004,
            1061503877,
            546368075,
            263116556,
            1281945677,
            1417850195,
            1127507743,
            1727180827,
            1017682338,
            1071620844,
            1739078074,
            947613573,
            1203005699,
            1451306892,
            2100398819,
            575927986,
            864153932,
            2025978504,
            1607277231,
            1588743815,
            1792600592,
            1307895453,
            1675684254,
            1811588772,
            941184293,
            1833602986,
            1505738938,
            803331382,
            1477879545,
            72728207,
            672401291,
            1363366364
        };
    }
}
