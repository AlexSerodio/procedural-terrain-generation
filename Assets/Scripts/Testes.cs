using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testes : MonoBehaviour
{

    public TerrainData terrainData;

    // Start is called before the first frame update
    void Start()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        // Debug.Log(5.0f / (int)2);
    }
}
