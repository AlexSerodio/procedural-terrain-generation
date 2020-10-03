using Generation.Terrain.Evaluation;
using Generation.Terrain.Physics.Erosion.GPU;
using Generation.Terrain.Physics.Erosion;
using Generation.Terrain.Procedural;
using Generation.Terrain.Utils;
using System;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class GeneralTerrainComponent : BaseComponent
    {
        public int smoothAmount;

        public ComputeShader shader;
        private DiamondSquare diamondSquare;
        ThermalErosion thermalErosion;
        ThermalErosionGPU thermalErosionGPU;

        public void SmoothTerrain(int iterations)
        {
            float[,] heightmap = GetTerrainHeight();

            Smooth.Apply(heightmap, iterations);

            UpdateTerrainHeight(heightmap);
        }

        public void ResetTerrain()
        {
            float[,] heightmap = new float[xSize, zSize];

            UpdateTerrainHeight(heightmap);
        }

        public override void UpdateComponent()
        {
            throw new System.NotImplementedException();
        }

        public void Compare()
        {
            int resolution = 1025;
            float talus = 3f / (float)resolution;
            float factor = 0.5f;
            int seed = -848589047;
            int iterations = 100;

            float[,] heightmapCPU = new float[resolution, resolution];
            float[,] heightmapGPU = new float[resolution, resolution];

            diamondSquare = new DiamondSquare(0, seed);
            diamondSquare.Apply(heightmapCPU);

            diamondSquare = new DiamondSquare(0, seed);
            diamondSquare.Apply(heightmapGPU);

            Debug.Log($"Erosion Score Diamond-Square: {ErosionScore.Evaluate(heightmapCPU)}");
            Debug.Log($"Diamonds equal: {AreEqual(heightmapCPU, heightmapGPU)} | Normalized: {IsNormalizes(heightmapCPU) && IsNormalizes(heightmapCPU)}");

            thermalErosion = new ThermalErosion();
            thermalErosion.Erode(heightmapCPU, talus, factor, iterations);

            thermalErosionGPU = new ThermalErosionGPU(shader);
            thermalErosionGPU.Erode(heightmapGPU, talus, factor, iterations);

            Debug.Log($"Erosion Score Thermal Erosion CPU: {ErosionScore.Evaluate(heightmapCPU)} | GPU: {ErosionScore.Evaluate(heightmapGPU)}");
            Debug.Log($"Erosions equal: {AreEqual(heightmapCPU, heightmapGPU)} | Normalized: {IsNormalizes(heightmapCPU) && IsNormalizes(heightmapCPU)}");

            UpdateTerrainHeight(heightmapCPU);
        }

        // 100 -> 0.03f
        // 200 -> 0.04f
        // 300 -> 0.04f
        // 400 -> 0.04f
        // 500 -> 0.04f
        private bool AreEqual(float[,] actual, float[,] expected, float allowedDifference = 0.03f)
        {
            if(actual.GetLength(0) != expected.GetLength(0) || actual.GetLength(1) != expected.GetLength(1))
                return false;

            for (int i = 0; i < actual.GetLength(0); i++)
            {
                for (int j = 0; j < actual.GetLength(1); j++)
                {
                    if(Math.Abs(actual[i, j] - expected[i, j]) > allowedDifference)
                        return false;
                }
            }

            return true;
        }

        private bool IsNormalizes(float[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if(matrix[i, j] > 1f || matrix[i, j] < 0f)
                        return false;
                }
            }

            return true;
        }
    }
}
