using Terrain.Evaluation;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Terrain.Utils
{
    public class HeightmapLoader
    {
        public float[,] Load(Texture2D image, float[,] heightmap)
        {
            int terrainWidth = heightmap.GetLength(0);

            Color[] map = SameSize(image, terrainWidth) ? image.GetPixels() : Resize(image, terrainWidth);

            for (int y = 0; y < terrainWidth; y++)
                for (int x = 0; x < terrainWidth; x++)
                    heightmap[y, x] = map[y * terrainWidth + x].grayscale;

            LogResults(heightmap);

            return heightmap;
        }

        private bool SameSize(Texture2D image, int terrainWidth)
        {
            return terrainWidth == image.width && image.height == image.width;
        }

        private Color[] Resize(Texture2D image, int terrainWidth)
        {
            Color[] pixels = image.GetPixels();
            Color[] map = new Color[terrainWidth * terrainWidth];

            float ratioX = (1.0f / ((float)terrainWidth / (image.width - 1)));
            float ratioY = (1.0f / ((float)terrainWidth / (image.height - 1)));
            for (int y = 0; y < terrainWidth; y++)
            {
                int yy = Mathf.FloorToInt(y * ratioY);
                int y1 = yy * image.width;
                int y2 = (yy + 1) * image.width;
                int yw = y * terrainWidth;

                for (int x = 0; x < terrainWidth; x++)
                {
                    int xx = Mathf.FloorToInt(x * ratioX);
                    Color bl = pixels[y1 + xx];
                    Color br = pixels[y1 + xx + 1];
                    Color tl = pixels[y2 + xx];
                    Color tr = pixels[y2 + xx + 1];
                    float xLerp = x * ratioX - xx;

                    map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - (float)yy);
                }
            }

            return map;
        }

        private void LogResults(float[,] heightmap)
        {
            string erosionScore = ErosionScore.Evaluate(heightmap).ToString();
            string benfordsLaw = BenfordsLaw.Evaluate(heightmap);

            EvaluationLogger.RecordValue("erosion_score", heightmap.GetLength(0), erosionScore);
            EvaluationLogger.RecordValue("benfords_law", heightmap.GetLength(0), benfordsLaw);
        }
    }
}
