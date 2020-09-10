using UnityEngine;

namespace Generation.Terrain.Procedural
{
    public class MidpointDisplacement : ITerrainModifier
    {
        public float HeightMin { get; set; }
        public float HeightMax { get; set; }
        public float HeightDampenerPower { get; set; }
        public float Roughness { get; set; }

        public MidpointDisplacement()
        {
            HeightMin = -2f;
            HeightMax = 2f;
            HeightDampenerPower = 2.0f;
            Roughness = 2.0f;
        }

        // TODO: refactor so that it is no longer necessary to use the UnityEngine namespace
        public void Apply(float[,] heights)
        {
            int width = heights.GetLength(0) - 1;
            int size = width;
            float heightMin = HeightMin;
            float heightMax = HeightMax;
            float heightDampener = (float)Mathf.Pow(HeightDampenerPower, -1 * Roughness);

            int mx, my;
            int left, right, up, down;

            float average = 0f;
            int halfSize = 0;

            while (size > 0)
            {
                halfSize = size / 2;

                for (int x = 0; x < width; x += size)
                {
                    for (int y = 0; y < width; y += size)
                    {
                        // diamond step
                        average = heights[x, y];
                        average += heights[x + size, y];
                        average += heights[x, y + size];
                        average += heights[x + size, y + size];
                        average /= 4.0f;

                        average += Offset(heightMin, heightMax);
                        heights[x + halfSize, y + halfSize] = average;

                        // square step
                        mx = x + halfSize;
                        my = y + halfSize;

                        right = mx + size;
                        up = my + size;
                        left = mx - size;
                        down = my - size;

                        // if pmid values are off the limit, ignore them
                        if (left <= 0 || down <= 0 || right >= width - 1 || up >= width - 1)
                            continue;

                        // Calculate the square value for the bottom side  
                        average = (heights[mx, my] +
                                    heights[x, y] +
                                    heights[mx, down] +
                                    heights[x + size, y]) / 4.0f;
                        heights[mx, y] = average + Offset(heightMin, heightMax);

                        // Calculate the square value for the top side
                        average = (heights[x, y + size] +
                                    heights[mx, my] +
                                    heights[x + size, y + size] +
                                    heights[mx, up]) / 4.0f;
                        heights[mx, y + size] = average + Offset(heightMin, heightMax);

                        // Calculate the square value for the left side
                        average = (heights[x, y] +
                                    heights[left, my] +
                                    heights[x, y + size] +
                                    heights[mx, my]) / 4.0f;
                        heights[x, my] = average + Offset(heightMin, heightMax);

                        // Calculate the square value for the right side
                        average = (heights[mx, y] +
                                    heights[mx, my] +
                                    heights[x + size, y + size] +
                                    heights[right, my]) / 4.0f;
                        heights[x + size, my] = average + Offset(heightMin, heightMax);
                    }
                }

                // update step
                size = size / 2;
                heightMin *= heightDampener;
                heightMax *= heightDampener;
            }
        }

        private float Offset(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
