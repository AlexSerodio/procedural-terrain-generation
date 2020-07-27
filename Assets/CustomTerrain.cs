using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour
{
    [Header("Keep it between 0 and 1")]
    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    public bool resetTerrain = true;

    // Perlin Noise
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;

    // TODO: move class to another file
    [System.Serializable]
    public class PerlinParameters
    {
        public float scaleX = 0.01f;
        public float scaleY = 0.01f;
        public int octaves = 3;
        public float persistance = 8;
        public float heightScale = 0.09f;
        public int offsetX = 0;
        public int offsetY = 0;
        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>() { new PerlinParameters() };

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

    //SPLATMAPS ---------------------------------------------
    [System.Serializable]
    public class SplatHeights
    {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 1.5f;
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
        public float splatOffset = 0.1f;
        public float splatNoiseXScale = 0.01f;
        public float splatNoiseYScale = 0.01f;
        public float splatNoiseScaler = 0.1f;
        public bool remove = false;
    }

    public List<SplatHeights> splatHeights = new List<SplatHeights>() {  new SplatHeights() };

    // VEGETATION ---------------------------------------------
    [System.Serializable]
    public class Vegetation
    {
        public GameObject mesh;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float minScale = 0.5f;
        public float maxScale = 1.0f;
        public Color colour1 = Color.white;
        public Color colour2 = Color.white;
        public Color lightColour = Color.white;
        public float minRotation = 0;
        public float maxRotation = 360;
        public float density = 0.5f;
        public bool remove = false;
    }

    public List<Vegetation> vegetation = new List<Vegetation>() { new Vegetation() };

    public int maxTrees = 5000;
    public int treeSpacing = 5;

    // Details --------------------------------------------------------
    [System.Serializable]
    public class Detail {
        public GameObject prototype = null;
        public Texture2D prototypeTexture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 1;
        public Color dryColour = Color.white;
        public Color healthyColour = Color.white;
        public Vector2 heightRange = new Vector2(1, 1);
        public Vector2 widthRange = new Vector2(1, 1);
        public float noiseSpread = 0.5f;
        public float overlap = 0.01f;
        public float feather = 0.05f;
        public float density = 0.5f;
        public bool remove = false;
    }

    public List<Detail> details = new List<Detail>() { new Detail() };

    public int maxDetails = 5000;
    public int detailSpacing = 5;

    //Water Level -------------------------------------------
    public float waterHeight = 0.5f;
    public GameObject waterGO;
    public Material shoreLineMaterial;

    //EROSION ------------------------------------------------
    public enum ErosionType { Rain, Thermal, Tidal, River, Wind, Canyon }
    public ErosionType erosionType = ErosionType.Rain;
    public float erosionStrength = 0.1f;
    public float erosionAmount = 0.01f;
    public int springsPerRiver = 5;
    public float solubility = 0.01f;
    public int droplets = 10;
    public int erosionSmoothAmount = 5;

    //CLOUDS -----------------------------------------------
    public int numClouds = 1;
    public int particlesPerCloud = 50;
    public Vector3 cloudScaleMin;
    public Vector3 cloudScaleMax;
    public Material cloudMaterial;
    public Material cloudShadowMaterial;
    public float cloudStartSize = 5;
    public Color cloudColour = Color.white;
    public Color cloudLining = Color.grey;
    public float cloudMinSpeed = 0.01f;
    public float cloudMaxSpeed = 0.1f;
    public float cloudRange = 500.0f;

    public Terrain terrain;
    public TerrainData terrainData;

    private SerializedObject tagManager;

    private string[] tags = {
        "Terrain",
        "Cloud",
        "Shore"
    };

    void OnEnable()
    {
        Debug.Log("Initializing Terrain Data");
        terrain = GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
    }

    public enum TagType { Tag, Layer}
    [SerializeField]
    int terrainLayer = -1;
    void Awake()
    {
        tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        SerializedProperty tagsProperty = tagManager.FindProperty("tags");

        foreach(var tag in tags)
            AddTag(tag, tagsProperty);

        tagManager.ApplyModifiedProperties();

        terrainLayer = AddLayer("Terrain");
        AddLayer("Sky");
        tagManager.ApplyModifiedProperties();

        //take this object
        this.gameObject.tag = "Terrain";
        this.gameObject.layer = terrainLayer;
    }

    private void AddTag(string newTag, SerializedProperty tagsProperty)
    {
        if(!TagExists(newTag, tagsProperty))
        {
            tagsProperty.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProperty = tagsProperty.GetArrayElementAtIndex(0);
            newTagProperty.stringValue = newTag;
        }
    }
    
    private bool TagExists(string tag, SerializedProperty tagsProperty)
    {
        for(int i = 0; i < tagsProperty.arraySize; i++)
        {
            SerializedProperty t = tagsProperty.GetArrayElementAtIndex(i);
            if(t.stringValue.Equals(tag))
                return true;
        }
        return false;
    }

    private int AddLayer(string newLayerName)
    {
        SerializedProperty layerProp = tagManager.FindProperty("layers");

        for(int i = 0; i < layerProp.arraySize; i++)
        {
            SerializedProperty layer = layerProp.GetArrayElementAtIndex(i);
            if(layer.stringValue.Equals(tag))
                return i;
        }

        int userLayerStartIndex = 8;
        for (int j = userLayerStartIndex; j < layerProp.arraySize; j++)
        {
            SerializedProperty newLayer = layerProp.GetArrayElementAtIndex(j);
            //add layer in next empty slot
            if (newLayer.stringValue == "")
            {
                newLayer.stringValue = newLayerName;
                return j;
            }
        }
        return -1;
    }

    private float[,] GetHeightMap()
    {
        if (!resetTerrain)
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        else
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
    }

    public void GenerateClouds()
    {
        GameObject cloudManager = GameObject.Find("CloudManager");
        if (!cloudManager)
        {
            cloudManager = new GameObject();
            cloudManager.name = "CloudManager";
            cloudManager.AddComponent<CloudManager>();
            cloudManager.transform.position = this.transform.position;
        }

        GameObject[] allClouds = GameObject.FindGameObjectsWithTag("Cloud");
        for (int i = 0; i < allClouds.Length; i++)
            DestroyImmediate(allClouds[i]);

        for (int c = 0; c < numClouds; c++)
        {
            GameObject cloudObject = new GameObject();
            cloudObject.name = "Cloud" + c;
            cloudObject.tag = "Cloud";

            cloudObject.transform.rotation = cloudManager.transform.rotation;
            cloudObject.transform.position = cloudManager.transform.position;
            CloudController cc = cloudObject.AddComponent<CloudController>();
            cc.lining = cloudLining;
            cc.colour = cloudColour;
            cc.numberOfParticles = particlesPerCloud;
            cc.minSpeed = cloudMinSpeed;
            cc.maxSpeed = cloudMaxSpeed;
            cc.distance = cloudRange;

            ParticleSystem cloudSystem = cloudObject.AddComponent<ParticleSystem>();
            Renderer cloudRend = cloudObject.GetComponent<Renderer>();
            cloudRend.material = cloudMaterial;

            cloudObject.layer = LayerMask.NameToLayer("Sky");
            GameObject cloudProjector = new GameObject();
            cloudProjector.name = "Shadow";
            cloudProjector.transform.position = cloudObject.transform.position;
            cloudProjector.transform.forward = Vector3.down;
            cloudProjector.transform.parent = cloudObject.transform;

            if(UnityEngine.Random.Range(0,10) < 5)
            {
                Projector cp = cloudProjector.AddComponent<Projector>();
                cp.material = cloudShadowMaterial;
                cp.farClipPlane = terrainData.size.y;
                int skyLayerMask = 1 << LayerMask.NameToLayer("Sky");
                int waterLayerMask = 1 << LayerMask.NameToLayer("Water");
                cp.ignoreLayers = skyLayerMask | waterLayerMask;
                cp.fieldOfView = 20.0f;
            }

            cloudRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            cloudRend.receiveShadows = false;
            ParticleSystem.MainModule main = cloudSystem.main;
            main.loop = false;
            main.startLifetime = Mathf.Infinity;
            main.startSpeed = 0;
            main.startSize = cloudStartSize;
            main.startColor = Color.white;

            var emission = cloudSystem.emission;
            emission.rateOverTime = 0; //all at once
            emission.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0.0f, (short)particlesPerCloud) });

            var shape = cloudSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            Vector3 newScale = new Vector3(UnityEngine.Random.Range(cloudScaleMin.x, cloudScaleMax.x),
                                           UnityEngine.Random.Range(cloudScaleMin.y, cloudScaleMax.y),
                                           UnityEngine.Random.Range(cloudScaleMin.z, cloudScaleMax.z));
            shape.scale = newScale;

            cloudObject.transform.parent = cloudManager.transform;
            cloudObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Erode()
    {
        switch (erosionType)
        {
            case ErosionType.Rain:
                RainErosion();
                break;
            case ErosionType.Thermal:
                ThermalErosion(terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution), 0.003f, 0.045f);
                break;
            case ErosionType.Tidal:
                TidalErosion();
                break;
            case ErosionType.River:
                RiverErosion();
                break;
            case ErosionType.Wind:
                WindErosion();
                break;
            case ErosionType.Canyon:
                DigCanyon();
                break;
        }

        smoothAmount = erosionSmoothAmount;
        Smooth();
    }

    /// <summary>
    /// Generate droplets in random positions and subtract the erosionStrength from the terrain in these positions.
    /// </summary>
    private void RainErosion()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int i = 0; i < droplets; i++)
        {
            int randomX = UnityEngine.Random.Range(0, terrainData.heightmapResolution);
            int randomY = UnityEngine.Random.Range(0, terrainData.heightmapResolution);
            heightMap[randomX, randomY] -= erosionStrength;
        }

        terrainData.SetHeights(0, 0, heightMap);
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

    /// <summary>
    /// Foreach position, check if the current height is less than the water height and if it's neighbors height is bigger than it.
    /// If so, update both heights to the water height, creating a slope on the beach.
    /// </summary>
    private void TidalErosion()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbors = GenerateNeighbors(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbors)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        heightMap[x, y] = waterHeight;
                        heightMap[(int)n.x, (int)n.y] = waterHeight;
                    }
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    /// <summary>
    /// Creates random droplets and foreach droplet creates a river running down the terrain, eroding the terrain
    /// and loading terrain material with it.
    /// </summary>
    private void RiverErosion()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,] erosionMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        for (int i = 0; i < droplets; i++)
        {
            int dropletX = UnityEngine.Random.Range(0, terrainData.heightmapResolution);
            int dropletY = UnityEngine.Random.Range(0, terrainData.heightmapResolution);
            Vector2 dropletPosition = new Vector2(dropletX, dropletY);

            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
            for (int j = 0; j < springsPerRiver; j++)
            {
                erosionMap = RunRiver(dropletPosition, heightMap, erosionMap, terrainData.heightmapResolution, terrainData.heightmapResolution);
            }
        }

        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                if (erosionMap[x, y] > 0)
                    heightMap[x, y] -= erosionMap[x, y];
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    private float[,] RunRiver(Vector2 dropletPosition, float[,] heightMap, float[,] erosionMap, int width, int height)
    {
        while (erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0)
        {
            List<Vector2> neighbors = GenerateNeighbors(dropletPosition, width, height);
            neighbors.Shuffle();
            bool foundLower = false;
            foreach (Vector2 n in neighbors)
            {
                if (heightMap[(int)n.x, (int)n.y] < heightMap[(int)dropletPosition.x, (int)dropletPosition.y])
                {
                    erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] - solubility;
                    dropletPosition = n;
                    foundLower = true;
                    break;
                }
            }
            if (!foundLower)
            {
                erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= solubility;
            }
        }
        return erosionMap;
    }

    private void WindErosion()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float windDirectionDegree = 30;
        float sinAngle = -Mathf.Sin(Mathf.Deg2Rad * windDirectionDegree);
        float cosAngle = Mathf.Cos(Mathf.Deg2Rad * windDirectionDegree);

        const float carriedMaterial = 0.001f;
        const float noiseScaler = 0.06f;

        const int interval = 10;

        for (int y = -(height - 1)*2; y <= height*2; y += interval)
        {
            for (int x = -(width - 1)*2; x <= width*2; x++)
            {
                float thisNoise = (float)Mathf.PerlinNoise(x * noiseScaler, y * noiseScaler) * 20 * erosionStrength;
                int nx = (int)x;
                int ny = (int)y + (interval/2) + (int)thisNoise;
                int digy = (int)y + (int)thisNoise;

                Vector2 digCoords = new Vector2(x * cosAngle - digy * sinAngle, digy * cosAngle + x * sinAngle);
                Vector2 pileCoords = new Vector2(nx * cosAngle - ny * sinAngle, ny * cosAngle + nx * sinAngle);

                if (!(pileCoords.x < 0 || pileCoords.x > (width - 1) || pileCoords.y < 0 || 
                      pileCoords.y > (height - 1) ||
                     (int)digCoords.x < 0 || (int)digCoords.x > (width - 1) ||
                     (int)digCoords.y < 0 || (int)digCoords.y > (height - 1)))
                {
                    heightMap[(int)digCoords.x, (int)digCoords.y] -= carriedMaterial;
                    heightMap[(int)pileCoords.x, (int)pileCoords.y] += carriedMaterial;
                }

            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    float[,] tempHeightMap;
    private void DigCanyon()
    {
        float digDepth = 0.05f;
        float bankSlope = 0.001f;
        float maxDepth = 0;
        tempHeightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int cx = 1;
        int cy = UnityEngine.Random.Range(10, terrainData.heightmapResolution - 10);
        while (cy >= 0 && cy < terrainData.heightmapResolution && cx > 0 && cx < terrainData.heightmapResolution)
        {
            CanyonCrawler(cx, cy, tempHeightMap[cx, cy] - digDepth, bankSlope, maxDepth);
            cx = cx + UnityEngine.Random.Range(1, 3);
            cy = cy + UnityEngine.Random.Range(-2, 3);
        }
        terrainData.SetHeights(0, 0, tempHeightMap);
    }

    private void CanyonCrawler(int x, int y, float height, float slope, float maxDepth)
    {
        if (x < 0 || x >= terrainData.heightmapResolution)  //off x range of map
            return; 
        if (y < 0 || y >= terrainData.heightmapResolution)  //off y range of map
            return; 
        if (height <= maxDepth)                             //if hit lowest level
            return; 
        if (tempHeightMap[x, y] <= height)                  //if run into lower elevation
            return; 

        tempHeightMap[x, y] = height;

        CanyonCrawler(x + 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x + 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y - 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
    }

    public void PlantVegetation()
    {
        PrepareTreesPrototypes();

        List<TreeInstance> allVegetation = new List<TreeInstance>();
        for (int z = 0; z < terrainData.size.z; z += treeSpacing)
        {
            for (int x = 0; x < terrainData.size.x; x += treeSpacing)
            {
                for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > vegetation[tp].density) 
                        break;

                    float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                    float thisHeightStart = vegetation[tp].minHeight;
                    float thisHeightEnd = vegetation[tp].maxHeight;
                    
                    float steepness = terrainData.GetSteepness(x / (float)terrainData.size.x, z / (float)terrainData.size.z);

                    if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                        (steepness >= vegetation[tp].minSlope && steepness <= vegetation[tp].maxSlope))
                    {
                        TreeInstance instance = new TreeInstance();
                        instance.position = new Vector3((x + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.x,
                                                        terrainData.GetHeight(x, z) / terrainData.size.y,
                                                        (z + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.z);

                        Vector3 treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                            instance.position.y * terrainData.size.y,
                            instance.position.z * terrainData.size.z) + this.transform.position;

                        RaycastHit hit;
                        int layerMask = 1 << terrainLayer;
                        Vector3 heightCastOffset = new Vector3(0, 10, 0);
                        if (Physics.Raycast(treeWorldPos + heightCastOffset, -Vector3.up, out hit, 100, layerMask)  ||
                            Physics.Raycast(treeWorldPos - heightCastOffset, Vector3.up, out hit, 100, layerMask))
                        {
                            float treeHeight = (hit.point.y - this.transform.position.y) / terrainData.size.y;
                            instance.position = new Vector3(instance.position.x, treeHeight, instance.position.z);
                            
                            instance.rotation = UnityEngine.Random.Range(vegetation[tp].minRotation, vegetation[tp].maxRotation);
                            instance.prototypeIndex = tp;
                            instance.color = Color.Lerp(vegetation[tp].colour1, vegetation[tp].colour2, UnityEngine.Random.Range(0.0f,1.0f));
                            instance.lightmapColor = Color.white;
                            float randomScale = UnityEngine.Random.Range(vegetation[tp].minScale, vegetation[tp].maxScale);
                            instance.heightScale = randomScale;
                            instance.widthScale = randomScale;

                            allVegetation.Add(instance);
                            if (allVegetation.Count >= maxTrees) goto TREESDONE;
                        }

                    }
                }
            }
        }
    TREESDONE:
        terrainData.treeInstances = allVegetation.ToArray();
    }

    private void PrepareTreesPrototypes()
    {
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[vegetation.Count];
        int tIndex = 0;
        foreach (Vegetation t in vegetation)
        {
            newTreePrototypes[tIndex] = new TreePrototype();
            newTreePrototypes[tIndex].prefab = t.mesh;
            tIndex++;
        }
        terrainData.treePrototypes = newTreePrototypes;
    }

    public void AddNewVegetation()
    {
        vegetation.Add(new Vegetation());
    }

    public void RemoveVegetation()
    {
        List<Vegetation> keptVegetation = new List<Vegetation>();
        for (int i = 0; i < vegetation.Count; i++)
        {
            if (!vegetation[i].remove)
            {
                keptVegetation.Add(vegetation[i]);
            }
        }
        if (keptVegetation.Count == 0)          // don't want to keep any
        {
            keptVegetation.Add(vegetation[0]);  // add at least 1
        }
        vegetation = keptVegetation;
    }

    public void AddDetails() {
        PrepareDetailsPrototypes();

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        for(int i = 0; i < terrainData.detailPrototypes.Length; ++i)
        {
            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];

            for(int y = 0; y < terrainData.detailHeight; y += detailSpacing)
            {
                for(int x = 0; x < terrainData.detailWidth; x += detailSpacing)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > details[i].density)
                        continue;

                    int xHM = (int)(x / (float)terrainData.detailWidth * terrainData.heightmapResolution);
                    int yHM = (int)(y / (float)terrainData.detailHeight * terrainData.heightmapResolution);

                    float thisNoise = Utils.Map(Mathf.PerlinNoise(x * details[i].feather, y * details[i].feather), 0, 1, 0.5f, 1);
                    float thisHeightStart = details[i].minHeight * thisNoise - details[i].overlap * thisNoise;
                    float thisHeightEnd = details[i].maxHeight * thisNoise + details[i].overlap * thisNoise;

                    float thisHeight = heightMap[yHM, xHM];
                    float steepness = terrainData.GetSteepness(xHM / (float)terrainData.size.x, yHM / (float)terrainData.size.z);
                    
                    if((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                        (steepness >= details[i].minSlope && steepness <= details[i].maxSlope)) {
                        detailMap[y, x] = 1;
                    }
                }
            }
            terrainData.SetDetailLayer(0, 0, i, detailMap);
        }
    }

    private void PrepareDetailsPrototypes()
    {
        DetailPrototype[] newDetailPrototypes;
        newDetailPrototypes = new DetailPrototype[details.Count];
        int dIndex = 0;
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        foreach (Detail d in details) {
            newDetailPrototypes[dIndex] = new DetailPrototype();
            newDetailPrototypes[dIndex].prototype = d.prototype;
            newDetailPrototypes[dIndex].prototypeTexture = d.prototypeTexture;
            newDetailPrototypes[dIndex].healthyColor = d.healthyColour;
            newDetailPrototypes[dIndex].dryColor = d.dryColour;
            newDetailPrototypes[dIndex].minHeight = d.heightRange.x;
            newDetailPrototypes[dIndex].maxHeight = d.heightRange.y;
            newDetailPrototypes[dIndex].minWidth = d.widthRange.x;
            newDetailPrototypes[dIndex].maxWidth = d.widthRange.y;
            newDetailPrototypes[dIndex].noiseSpread = d.noiseSpread;

            if (newDetailPrototypes[dIndex].prototype) {
                newDetailPrototypes[dIndex].usePrototypeMesh = true;
                newDetailPrototypes[dIndex].renderMode = DetailRenderMode.VertexLit;
            } else {
                newDetailPrototypes[dIndex].usePrototypeMesh = false;
                newDetailPrototypes[dIndex].renderMode = DetailRenderMode.GrassBillboard;
            }
            dIndex++;
        }
        terrainData.detailPrototypes = newDetailPrototypes;
    }

    public void AddNewDetails() {
        details.Add(new Detail());
    }

    public void RemoveDetails() {
        List<Detail> keptDetails = new List<Detail>();
        for (int i = 0; i < details.Count; ++i) {
            if (!details[i].remove) {
                keptDetails.Add(details[i]);
            }
        }
        if (keptDetails.Count == 0) {    // Don't want to keep any
            keptDetails.Add(details[0]);  // Add at least one;
        }
        details = keptDetails;
    }   

    public void AddWater()
    {
        GameObject water = GameObject.Find("Water");
        if (!water)
        {
            water = Instantiate(waterGO, this.transform.position, this.transform.rotation);
            water.name = "Water";
        }
        water.transform.position = this.transform.position + 
            new Vector3(terrainData.size.x / 2, waterHeight * terrainData.size.y, terrainData.size.z / 2);
        water.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
    }

    public void DrawShoreline()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                                                    terrainData.heightmapResolution);

        float scaleFactor = 10.0f;
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                //find spot on shore
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbors = GenerateNeighbors(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbors)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        quadObject.transform.localScale *= scaleFactor;

                        quadObject.transform.position = this.transform.position +
                                        new Vector3(y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                                    waterHeight * terrainData.size.y,
                                                    x / (float)terrainData.heightmapResolution * terrainData.size.x);

                        quadObject.transform.LookAt(new Vector3(n.y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                                        waterHeight * terrainData.size.y, 
                                                        n.x / (float)terrainData.heightmapResolution * terrainData.size.x));    

                        quadObject.transform.Rotate(90, 0, 0);

                        quadObject.tag = "Shore";
                    }
                }
            }
        }

        GameObject[] shoreQuads = GameObject.FindGameObjectsWithTag("Shore");
        MeshFilter[] meshFilters = new MeshFilter[shoreQuads.Length];
        for (int m = 0; m < shoreQuads.Length; m++)
            meshFilters[m] = shoreQuads[m].GetComponent<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        GameObject currentShoreLine = GameObject.Find("ShoreLine");
        if (currentShoreLine)
            DestroyImmediate(currentShoreLine);

        GameObject shoreLine = new GameObject("ShoreLine");
        shoreLine.AddComponent<WaveAnimation>();
        shoreLine.transform.position = this.transform.position;
        shoreLine.transform.rotation = this.transform.rotation;
        
        MeshFilter thisMF = shoreLine.AddComponent<MeshFilter>();
        thisMF.mesh = new Mesh();
        
        shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        MeshRenderer r = shoreLine.AddComponent<MeshRenderer>();
        r.sharedMaterial = shoreLineMaterial;

        shoreLine.layer = LayerMask.NameToLayer("Water");

        for (int i = 0; i < shoreQuads.Length; i++)
            DestroyImmediate(shoreQuads[i]);
    }

    public void AddNewSplatHeight()
    {
        splatHeights.Add(new SplatHeights());
    }

    public void RemoveSplatHeight()
    {
        List<SplatHeights> keptSplatHeights = new List<SplatHeights>();
        for (int i = 0; i < splatHeights.Count; i++)
        {
            if (!splatHeights[i].remove)
                keptSplatHeights.Add(splatHeights[i]);
        }
        if (keptSplatHeights.Count == 0)            //don't want to keep any
            keptSplatHeights.Add(splatHeights[0]);  //add at least 1

        splatHeights = keptSplatHeights;
    }

    float GetSteepness(float[,] heightmap, int x, int y, int width, int height)
    {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;

        //if on the upper edge of the map find gradient by going backward.
        if (nx > width - 1) nx = x - 1;
        if (ny > height - 1) ny = y - 1;

        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;
        Vector2 gradient = new Vector2(dx, dy);

        float steep = gradient.magnitude;

        return steep;
    }

    public void SplatMaps()
    {
        TerrainLayer[] newTerrainLayers;
        newTerrainLayers = new TerrainLayer[splatHeights.Count];
        int spIndex = 0;
        foreach (SplatHeights sh in splatHeights)
        {
            newTerrainLayers[spIndex] = new TerrainLayer();
            newTerrainLayers[spIndex].diffuseTexture = sh.texture;
            newTerrainLayers[spIndex].tileOffset = sh.tileOffset;
            newTerrainLayers[spIndex].tileSize = sh.tileSize;
            newTerrainLayers[spIndex].diffuseTexture.Apply(true);
            spIndex++;
        }
        terrainData.terrainLayers = newTerrainLayers;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < splatHeights.Count; i++)
                {
                    float noise = Mathf.PerlinNoise(x * splatHeights[i].splatNoiseXScale, y * splatHeights[i].splatNoiseYScale) * splatHeights[i].splatNoiseScaler;
                    float overlapOffset = splatHeights[i].splatOffset + noise;

                    float thisHeightStart = splatHeights[i].minHeight - overlapOffset;
                    float thisHeightStop = splatHeights[i].maxHeight + overlapOffset;

                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight, x / (float)terrainData.alphamapWidth);

                    // float steepness = GetSteepness(heightMap, x, y, terrainData.heightmapResolution, terrainData.heightmapResolution);

                    if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop) &&
                        (steepness >= splatHeights[i].minSlope && steepness <= splatHeights[i].maxSlope))
                    {
                        splat[i] = 1;
                    }
                }
                NormalizeVector(splat);
                for (int j = 0; j < splatHeights.Count; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData); 
    }

    private void NormalizeVector(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
            total += v[i];

        for (int i = 0; i < v.Length; i++)
            v[i] /= total;
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

    public void Smooth()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress);

        for (int s = 0; s < smoothAmount; s++)
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
            EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress / smoothAmount);
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

        // heightMap[0, 0] = UnityEngine.Random.Range(0f, 0.2f);
        // heightMap[0, terrainData.heightmapResolution - 2] = UnityEngine.Random.Range(0f, 0.2f);
        // heightMap[terrainData.heightmapResolution - 2, 0] = UnityEngine.Random.Range(0f, 0.2f);
        // heightMap[terrainData.heightmapResolution - 2, terrainData.heightmapResolution - 2] = UnityEngine.Random.Range(0f, 0.2f);

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

    public void MatrixToString()
    {
        float[,] matrix = GetHeightMap();
     
        var content = new System.Text.StringBuilder();
        content.Append("{\n");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            content.Append("\t{");
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                content.Append(matrix[i, j].ToString("n6"));
                if(j < matrix.GetLength(1)-1)
                    content.Append(",");
                content.Append(" ");
            }
            content.Append("},\n");
        }
        content.Append("}");

        // Debug.Log(content.ToString());
        WriteString(content.ToString(), "teste");
    }

    private void WriteString(string content, string fileName)
    {
        string path = $"Assets/Samples/{fileName}.txt";

        //Write some text to the test.txt file
        System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true);
        writer.Write(content);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path); 
        TextAsset asset = Resources.Load(fileName) as TextAsset;
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
                heightMap[x, y] += Utils.fBM((x+perlinOffsetX) * perlinXScale, (y+perlinOffsetY) * perlinYScale, 
                                            perlinOctaves, perlinPersistance) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlinTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utils.fBM((x + p.offsetX) * p.scaleX, (y + p.offsetY) * p.scaleY,
                                                p.octaves, p.persistance) * p.heightScale;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void AddNewPerlin()
    {
        perlinParameters.Add(new PerlinParameters());
    }

    // TODO: change this to a filter method
    public void RemovePerlin()
    {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
        for (int i = 0; i < perlinParameters.Count; i++)
        {
            if (!perlinParameters[i].remove)
            {
                keptPerlinParameters.Add(perlinParameters[i]);
            }
        }
        if (keptPerlinParameters.Count == 0) //don't want to keep any
        {
            keptPerlinParameters.Add(perlinParameters[0]); //add at least 1
        }
        perlinParameters = keptPerlinParameters;
    }

    public void RandomTerrain()
    {
        float[,] heightMap = GetHeightMap();

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void LoadTexture()
    {
        float[,] heightMap = GetHeightMap();

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] += heightMapImage
                    .GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z))
                    .grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] = 0.1f;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SaveHeights(string filename)
    {
        var writer = new Generation.Terrain.Utils.ReadWriteTerrain(filename);

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        writer.WriteMatrix(heightMap);
    }

    public void LoadHeights(string filename)
    {
        var reader = new Generation.Terrain.Utils.ReadWriteTerrain(filename);

        float[,] heightMap = reader.ReadMatrix();
        terrainData.SetHeights(0, 0, heightMap);
    }
}
