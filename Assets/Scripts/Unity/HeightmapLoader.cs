using Generation.Terrain.Evaluation;
using TerrainGeneration.Analytics;
using UnityEngine;

public class HeightmapLoader
{
    public float[,] Load(Texture2D image, float[,] heightmap)
    {
        int w = image.width;
        int h = image.height;
        int w2 = heightmap.GetLength(0);

        Color[] map = SameSize(w, h, w2) ? image.GetPixels() : Resize(image, w, h, w2);

        for (int y = 0; y < w2; y++)
            for (int x = 0; x < w2; x++)
                heightmap[y, x] = map[y * w2 + x].grayscale;

        LogResults(heightmap);

        return heightmap;
    }

    private bool SameSize(int w, int h, int w2) => w2 == w && h == w;

    private Color[] Resize(Texture2D image, int w, int h, int w2)
    {
        Color[] pixels = image.GetPixels();
        Color[] map = new Color[w2 * w2];

        // Resize using nearest-neighbor scaling if texture has no filtering
        if (image.filterMode == FilterMode.Point)
        {
            float dx = (float)w / (float)w2;
            float dy = (float)h / (float)w2;
            for (int y = 0; y < w2; y++)
            {
                int thisY = Mathf.FloorToInt(dy * y) * w;
                int yw = y * w2;

                for (int x = 0; x < w2; x++)
                    map[yw + x] = pixels[Mathf.FloorToInt(thisY + dx * x)];
            }
        }
        // Otherwise resize using bilinear filtering
        else
        {
            float ratioX = (1.0f / ((float)w2 / (w - 1)));
            float ratioY = (1.0f / ((float)w2 / (h - 1)));
            for (int y = 0; y < w2; y++)
            {
                int yy = Mathf.FloorToInt(y * ratioY);
                int y1 = yy * w;
                int y2 = (yy + 1) * w;
                int yw = y * w2;

                for (int x = 0; x < w2; x++)
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
