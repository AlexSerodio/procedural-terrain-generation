using System;
using System.IO;
using Terrain.Evaluation;
using Terrain.Utils;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class HeightmapLoaderComponent : BaseComponent
    {
        public Texture2D texture;

        private HeightmapLoader loader = new HeightmapLoader();

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();

            loader.Load(texture, heightmap);

            Debug.Log($"Loaded Terrain: {ErosionScore.Evaluate(heightmap)} -> {BenfordsLaw.Evaluate(heightmap)}");
            
            UpdateTerrainHeight(heightmap);
        }

        public void export()
        {
            float[,] heightmap = GetTerrainHeight();
            string pathFile = "./Assets/TerrainGenerator/Heightmaps/" + DateTime.Now.ToString("dd-MM-yyyy-h-mm-ss") + ".png";
            SaveImageToDisk(pathFile, heightmap);
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
    }
}
