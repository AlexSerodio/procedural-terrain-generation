using Terrain;
using UnityEngine;

namespace Generation.Terrain.Procedural
{
    public enum VoronoiType
    {
        Linear,
        Power,
        Combined,
        SinPow
    }

    public class Voronoi : ITerrainModifier
    {
        public float DropOff { get; set; }
        public float FallOff { get; set; }
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }
        public int PeaksAmount { get; set; }
        public VoronoiType Type { get; set; }

        public Voronoi()
        {
            DropOff = 0.6f;
            FallOff = 0.2f;
            MinHeight = 0.6f;
            MaxHeight = 0.2f;
            PeaksAmount = 1;
            Type = VoronoiType.Linear;
        }

        // TODO: refactor so that it is no longer necessary to use the UnityEngine namespace
        public void Apply(float[,] heightmap)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);

            for (int p = 0; p < PeaksAmount; p++)
            {
                Vector2 peakLocation = new Vector2(
                    UnityEngine.Random.Range(0, width),
                    UnityEngine.Random.Range(0, height)
                );
                float peakHeight = UnityEngine.Random.Range(MinHeight, MaxHeight);

                if (heightmap[(int)peakLocation.x, (int)peakLocation.y] < peakHeight)
                    heightmap[(int)peakLocation.x, (int)peakLocation.y] = peakHeight;
                else
                    continue;

                float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(width, height));

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (!(x == peakLocation.x && y == peakLocation.y))
                        {
                            float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y));
                            float normalizedDistance = distanceToPeak / maxDistance;

                            float voronoiHeight = 0f;

                            switch (Type)
                            {
                                case VoronoiType.Linear:
                                    voronoiHeight = peakHeight - (normalizedDistance * FallOff);
                                    break;
                                case VoronoiType.Power:
                                    voronoiHeight = peakHeight - Mathf.Pow(normalizedDistance, DropOff) * FallOff;
                                    break;
                                case VoronoiType.Combined:
                                    voronoiHeight = peakHeight - (normalizedDistance * FallOff) - Mathf.Pow(normalizedDistance, DropOff);
                                    break;
                                case VoronoiType.SinPow:
                                    voronoiHeight = peakHeight - Mathf.Pow(normalizedDistance * 3, FallOff) - Mathf.Sin(normalizedDistance * 2 * Mathf.PI) / DropOff;
                                    break;
                            }

                            if (heightmap[x, y] < voronoiHeight)
                                heightmap[x, y] = voronoiHeight;
                        }
                    }
                }
            }
        }
    }
}
