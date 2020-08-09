using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour
{
    // Perlin Noise
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;

    // Voronoi
    public enum VoronoiType { Linear, Power, Combined, SinPow }

    public float voronoiDropOff = 0.6f;
    public float voronoiFallOff = 0.2f;
    public float voronoiMinHeight = 0.6f;
    public float voronoiMaxHeight = 0.2f;
    public int voronoiPeaks = 1;
    public VoronoiType voronoiType = VoronoiType.Linear;

    //Midpoint Displacement
    public float MPDHeightMin = -2f;
    public float MPDHeightMax = 2f;
    public float MPDHeightDampenerPower = 2.0f;
    public float MPDRoughness = 2.0f;

    public int smoothAmount = 2;


    // EROSION ------------------------------------------------
    public enum ErosionType { Thermal }
    public ErosionType erosionType = ErosionType.Thermal;
    public int erosionSmoothAmount = 5;

    public TerrainData terrainData;

    void OnEnable()
    {
        Debug.Log("Initializing Terrain Data");
        terrainData = Terrain.activeTerrain.terrainData;
    }

    private float[,] GetHeightMap()
    {
        return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }

    public void Erode()
    {
        switch (erosionType)
        {
            case ErosionType.Thermal:
                ThermalErosion(terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution), 0.003f, 0.045f);
                break;
        }

        Smooth(erosionSmoothAmount);
    }

    /// <summary>
    /// Foreach position, check if the height of it's neighbors is less than the current height plus he erosionStrength.
    /// If so, a percentage of the current position height is removed from the current position and added to the neighbor.
    /// </summary>
    private void ThermalErosion(float[,] heightMap, float _erosionStrength, float _erosionAmount)
    {
        // float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbors = GenerateNeighbors(thisLocation, heightMap.GetLength(0), heightMap.GetLength(1));
                
                foreach (Vector2 n in neighbors)
                {
                    if (heightMap[x, y] > heightMap[(int)n.x, (int)n.y] + _erosionStrength)
                    {
                        float currentHeight = heightMap[x, y];
                        heightMap[x, y] -= currentHeight * _erosionAmount;
                        heightMap[(int)n.x, (int)n.y] += currentHeight * _erosionAmount;
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    private List<Vector2> GenerateNeighbors(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbors = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbors.Contains(nPos))
                        neighbors.Add(nPos);
                }
            }
        }
        return neighbors;
    }

    public void Smooth(int iterations)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress);

        for (int s = 0; s < iterations; s++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    float totalHeight = heightMap[x, y];
                    List<Vector2> neighbors = GenerateNeighbors(new Vector2(x, y),
                                                                terrainData.heightmapResolution,
                                                                terrainData.heightmapResolution);
                    foreach (Vector2 neighbor in neighbors)
                        totalHeight += heightMap[(int)neighbor.x, (int)neighbor.y];

                    heightMap[x, y] = totalHeight / ((float)neighbors.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress / iterations);
        }
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }

    // also known as diamond square algorithm
    public void MidPointDisplacement()
    {
        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapResolution - 1;
        int squareSize = width;
        // float height = (float)squareSize / 2.0f * 0.01f;
        float heightMin = MPDHeightMin;
        float heightMax = MPDHeightMax;
        // float roughness = 2.0f;
        float heightDampener = (float)Mathf.Pow(MPDHeightDampenerPower, -1 * MPDRoughness);

        int endX, endY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD; // int pmidXLeft, pmidXRight, pmidYUp, pmidYDown;

        while (squareSize > 0)
        {
            // diamond step
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    endX = (x + squareSize);
                    endY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] +
                                                        heightMap[endX, y] +
                                                        heightMap[x, endY] +
                                                        heightMap[endX, endY]) / 4.0f +
                                                        UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            // square step
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    endX = (x + squareSize);
                    endY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL = (int)(midX - squareSize);
                    pmidYD = (int)(midY - squareSize);

                    // if pmid values are off the limit, ignore them
                    if (pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1) 
                        continue;

                    //Calculate the square value for the bottom side  
                    heightMap[midX, y] = (float)((heightMap[midX, midY] +
                                                    heightMap[x, y] +
                                                    heightMap[midX, pmidYD] +
                                                    heightMap[endX, y]) / 4.0f +
                                                    UnityEngine.Random.Range(heightMin, heightMax));

                    //Calculate the square value for the top side   
                    heightMap[midX, endY] = (float)((heightMap[x, endY] +
                                                    heightMap[midX, midY] +
                                                    heightMap[endX, endY] +
                                                    heightMap[midX, pmidYU]) / 4.0f +
                                                    UnityEngine.Random.Range(heightMin, heightMax));

                    //Calculate the square value for the left side   
                    heightMap[x, midY] = (float)((heightMap[x, y] +
                                                    heightMap[pmidXL, midY] +
                                                    heightMap[x, endY] +
                                                    heightMap[midX, midY]) / 4.0f +
                                                    UnityEngine.Random.Range(heightMin, heightMax));
                    
                    //Calculate the square value for the right side   
                    heightMap[endX, midY] = (float)((heightMap[midX, y] +
                                                    heightMap[midX, midY] +
                                                    heightMap[endX, endY] +
                                                    heightMap[pmidXR, midY]) / 4.0f +
                                                    UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            // update step
            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Voronoi()
    {
        float[,] heightMap = GetHeightMap();

        for (int p = 0; p < voronoiPeaks; p++)
        {
            Vector2 peakLocation = new Vector2(
                UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                UnityEngine.Random.Range(0, terrainData.heightmapResolution)
            );
            float peakHeight = UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight);

            if(heightMap[(int)peakLocation.x, (int)peakLocation.y] < peakHeight)
                heightMap[(int)peakLocation.x, (int)peakLocation.y] = peakHeight;
            else
                continue;

            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapResolution, terrainData.heightmapResolution));

            for(int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for(int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    if(!(x == peakLocation.x && y == peakLocation.y))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y));
                        float normalizedDistance = distanceToPeak / maxDistance;

                        float height = 0f;

                        switch(voronoiType)
                        {
                            case VoronoiType.Linear:
                                height = peakHeight - (normalizedDistance * voronoiFallOff);
                                break;
                            case VoronoiType.Power:
                                height = peakHeight - Mathf.Pow(normalizedDistance, voronoiDropOff) * voronoiFallOff;
                                break;
                            case VoronoiType.Combined:
                                height = peakHeight - (normalizedDistance * voronoiFallOff) - Mathf.Pow(normalizedDistance, voronoiDropOff);
                                break;
                            case VoronoiType.SinPow:
                                height = peakHeight - Mathf.Pow(normalizedDistance * 3, voronoiFallOff) - Mathf.Sin(normalizedDistance * 2 * Mathf.PI) / voronoiDropOff;
                                break;
                        }                        

                        if(heightMap[x, y] < height)
                            heightMap[x, y] = height;
                    }
                }   
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    // TODO: Rename this method to something like BrownianNoise and create another
    // function called PerlinNoise that uses the PerlinNoise function directly.  
    public void Perlin()
    {
        float[,] heightMap = GetHeightMap();

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                heightMap[x, y] += fBM((x+perlinOffsetX) * perlinXScale, (y+perlinOffsetY) * perlinYScale, 
                                            perlinOctaves, perlinPersistance) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    /// <summary>
    /// Fractal Brownian Motion.
    /// Expands the Perlin Noise function adding several octaves together, with each octave 
    /// added being larger than the previous one, based on the 'persistence' factor.
    /// </summary>
    public float fBM(float x, float y, int octave, float persistance)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octave; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }

        return total / maxValue;
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
                heightMap[x, z] = 0.1f;
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
}

public static class Utils
{
    /// <summary>
    /// Fisher-Yates Shuffle.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}