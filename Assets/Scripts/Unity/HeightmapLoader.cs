using UnityEngine;

public class HeightmapLoader
{
    public void Load(float[,] heightmap, Texture2D image)
    {
        int w = image.width;
        int h = image.height;
        int w2 = heightmap.GetLength(0);

        Color[] mapColors = image.GetPixels();
        Color[] map = new Color[w2 * w2];

        if (NotSameSize(w, h, w2))
            Resize(image, w, h, w2, mapColors, map);
        else
            map = mapColors;

        // Assign texture data to heightmap
        for (int y = 0; y < w2; y++)
            for (int x = 0; x < w2; x++)
                heightmap[y, x] = map[y * w2 + x].grayscale;
    }

    private bool NotSameSize(int w, int h, int w2) => w2 != w || h != w;

    private void Resize(Texture2D image, int w, int h, int w2, Color[] mapColors, Color[] map)
    {
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
                    map[yw + x] = mapColors[Mathf.FloorToInt(thisY + dx * x)];
            }
        }
        else    // Otherwise resize using bilinear filtering
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
                    Color bl = mapColors[y1 + xx];
                    Color br = mapColors[y1 + xx + 1];
                    Color tl = mapColors[y2 + xx];
                    Color tr = mapColors[y2 + xx + 1];
                    float xLerp = x * ratioX - xx;

                    map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - (float)yy);
                }
            }
        }
    }
}
