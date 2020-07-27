using UnityEngine;
using UnityEditor;
using EditorGUITable;
using System.IO;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
    private SerializedProperty randomHeightRange;
    private SerializedProperty heightMapScale;
    private SerializedProperty heightMapImage;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty voronoiType;
    SerializedProperty MPDHeightMin;
    SerializedProperty MPDHeightMax;
    SerializedProperty MPDHeightDampenerPower;
    SerializedProperty MPDRoughness;

    SerializedProperty smoothAmount;

    GUITableState splatMapTable;
    SerializedProperty splatHeights;

    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    GUITableState vegMapTable;
    SerializedProperty vegetation;
    SerializedProperty maxTrees;
    SerializedProperty treeSpacing;

    GUITableState detailMapTable;
    SerializedProperty detail;
    SerializedProperty maxDetails;
    SerializedProperty detailSpacing;

    SerializedProperty waterHeight;
    SerializedProperty waterGO;
    SerializedProperty shoreLineMaterial;

    SerializedProperty erosionType;
    SerializedProperty erosionStrength;
    SerializedProperty erosionAmount;
    SerializedProperty springsPerRiver;
    SerializedProperty solubility;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;

    SerializedProperty numClouds;
    SerializedProperty particlesPerCloud;
    SerializedProperty cloudScaleMin;
    SerializedProperty cloudScaleMax;
    SerializedProperty cloudMaterial;
    SerializedProperty cloudShadowMaterial;
    SerializedProperty cloudStartSize;
    SerializedProperty cloudColour;
    SerializedProperty cloudLining;
    SerializedProperty cloudMinSpeed;
    SerializedProperty cloudMaxSpeed;
    SerializedProperty cloudRange;

    Texture2D hmTexture;

    private bool showRandom = false;
    private bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;
    bool showSplatMaps = false;
    bool showVeg = false;
    bool showHeights = false;
    bool showDetail = false;
    bool showWater = false;
    bool showErosion = false;
    bool showClouds = false;

    void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        voronoiType = serializedObject.FindProperty("voronoiType");
        MPDHeightMin = serializedObject.FindProperty("MPDHeightMin");
        MPDHeightMax = serializedObject.FindProperty("MPDHeightMax");
        MPDHeightDampenerPower = serializedObject.FindProperty("MPDHeightDampenerPower");
        MPDRoughness = serializedObject.FindProperty("MPDRoughness");
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        splatMapTable = new GUITableState("splatMapTable");
        splatHeights = serializedObject.FindProperty("splatHeights");
        vegMapTable = new GUITableState("vegMapTable");
        vegetation = serializedObject.FindProperty("vegetation");
        maxTrees = serializedObject.FindProperty("maxTrees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");
        detailMapTable = new GUITableState("detailMapTable");
        detail = serializedObject.FindProperty("details");
        maxDetails = serializedObject.FindProperty("maxDetails");
        detailSpacing = serializedObject.FindProperty("detailSpacing");

        waterHeight = serializedObject.FindProperty("waterHeight");
        waterGO = serializedObject.FindProperty("waterGO");
        shoreLineMaterial = serializedObject.FindProperty("shoreLineMaterial");

        erosionType = serializedObject.FindProperty("erosionType");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        erosionAmount = serializedObject.FindProperty("erosionAmount");
        springsPerRiver = serializedObject.FindProperty("springsPerRiver");
        solubility = serializedObject.FindProperty("solubility");
        droplets = serializedObject.FindProperty("droplets");
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");

        numClouds = serializedObject.FindProperty("numClouds");
        particlesPerCloud = serializedObject.FindProperty("particlesPerCloud");
        cloudScaleMin = serializedObject.FindProperty("cloudScaleMin");
        cloudScaleMax = serializedObject.FindProperty("cloudScaleMax");
        cloudMaterial = serializedObject.FindProperty("cloudMaterial");
        cloudStartSize = serializedObject.FindProperty("cloudStartSize");
        cloudColour = serializedObject.FindProperty("cloudColour");
        cloudLining = serializedObject.FindProperty("cloudLining");
        cloudMinSpeed = serializedObject.FindProperty("cloudMinSpeed");
        cloudMaxSpeed = serializedObject.FindProperty("cloudMaxSpeed");
        cloudRange = serializedObject.FindProperty("cloudRange");
        cloudShadowMaterial = serializedObject.FindProperty("cloudShadowMaterial");

        hmTexture = new Texture2D(513, 513, TextureFormat.ARGB32, false);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CustomTerrain terrain = (CustomTerrain)target;
        EditorGUILayout.PropertyField(resetTerrain);

        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Apply Random Heights"))
                terrain.RandomTerrain();
        }

        showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
        if (showLoadHeights)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load Heights From Texture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if (GUILayout.Button("Load Texture"))
                terrain.LoadTexture();
        }

        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
        if (showPerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 0.1f, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 0.1f, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));

            if (GUILayout.Button("Perlin"))
                terrain.Perlin();
        }

        showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if (showMultiplePerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);

            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, perlinParameters);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                terrain.AddNewPerlin();

            if (GUILayout.Button("-"))
                terrain.RemovePerlin();

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Multiple Perlin"))
                terrain.MultiplePerlinTerrain();
        }

        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if (showVoronoi)
        {
            EditorGUILayout.IntSlider(voronoiPeaks, 1, 10, new GUIContent("Peak Count"));
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("Falloff"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("Dropoff"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.PropertyField(voronoiType);
            if (GUILayout.Button("Voronoi"))
                terrain.Voronoi();
        }

        showMPD = EditorGUILayout.Foldout(showMPD, "Midpoint Displacement");
        if (showMPD)
        {
            EditorGUILayout.PropertyField(MPDHeightMin);
            EditorGUILayout.PropertyField(MPDHeightMax);
            EditorGUILayout.PropertyField(MPDHeightDampenerPower);
            EditorGUILayout.PropertyField(MPDRoughness);
            if (GUILayout.Button("MPD"))
                terrain.MidPointDisplacement();
        }

        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
        if (showSplatMaps)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, splatHeights);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                terrain.AddNewSplatHeight();

            if (GUILayout.Button("-"))
                terrain.RemoveSplatHeight();

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply SplatMaps"))
                terrain.SplatMaps();
        }

        showVeg = EditorGUILayout.Foldout(showVeg, "Vegetation");
        if (showVeg)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Vegetation", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxTrees, 0, 10000, new GUIContent("Maximum Trees"));
            EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Trees Spacing"));
            vegMapTable = GUITableLayout.DrawTable(vegMapTable,
                                        serializedObject.FindProperty("vegetation"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                terrain.AddNewVegetation();

            if (GUILayout.Button("-"))
                terrain.RemoveVegetation();

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Vegetation"))
                terrain.PlantVegetation();
        }

        showDetail = EditorGUILayout.Foldout(showDetail, "Details");
        if (showDetail)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Detail", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxDetails, 0, 10000, new GUIContent("Maximum Details"));
            EditorGUILayout.IntSlider(detailSpacing, 1, 20, new GUIContent("Detail Spacing"));
            detailMapTable = GUITableLayout.DrawTable(detailMapTable,
                serializedObject.FindProperty("details"));

            terrain.GetComponent<Terrain>().detailObjectDistance = maxDetails.intValue;

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
                terrain.AddNewDetails();
            if (GUILayout.Button("-"))
                terrain.RemoveDetails();

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Details"))
                terrain.AddDetails();
        }

        showWater = EditorGUILayout.Foldout(showWater, "Water");
        if (showWater)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Water", EditorStyles.boldLabel);
            EditorGUILayout.Slider(waterHeight, 0, 1, new GUIContent("Water Height"));
            EditorGUILayout.PropertyField(waterGO);

            if (GUILayout.Button("Add Water"))
                terrain.AddWater();

            EditorGUILayout.PropertyField(shoreLineMaterial);
            if (GUILayout.Button("Add Shoreline"))
                terrain.DrawShoreline();
        }

        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.Slider(erosionStrength, 0, 1f, new GUIContent("Erosion Strength"));
            EditorGUILayout.Slider(erosionAmount, 0, 1, new GUIContent("Erosion Amount"));
            EditorGUILayout.IntSlider(droplets, 0, 500, new GUIContent("Droplets"));
            EditorGUILayout.Slider(solubility, 0.001f, 1, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(springsPerRiver, 0, 20, new GUIContent("Springs Per River"));
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth Amount"));

            if (GUILayout.Button("Erode"))
                terrain.Erode();
        }

        showClouds = EditorGUILayout.Foldout(showClouds, "Clouds");
        if (showClouds)
        {
            EditorGUILayout.PropertyField(numClouds, new GUIContent("Number of Clouds"));
            EditorGUILayout.PropertyField(particlesPerCloud, new GUIContent("Particles Per Clouds"));
            EditorGUILayout.PropertyField(cloudStartSize, new GUIContent("Cloud Particle Size"));
            EditorGUILayout.PropertyField(cloudScaleMin, new GUIContent("Min Size"));
            EditorGUILayout.PropertyField(cloudScaleMax, new GUIContent("Max Size"));
            EditorGUILayout.PropertyField(cloudMaterial, true);
            EditorGUILayout.PropertyField(cloudShadowMaterial, true);
            EditorGUILayout.PropertyField(cloudColour, new GUIContent("Colour"));
            EditorGUILayout.PropertyField(cloudLining, new GUIContent("Lining"));
            EditorGUILayout.PropertyField(cloudMinSpeed, new GUIContent("Min Speed"));
            EditorGUILayout.PropertyField(cloudMaxSpeed, new GUIContent("Max Speed"));
            EditorGUILayout.PropertyField(cloudRange, new GUIContent("Distance Travelled"));

            if (GUILayout.Button("Generate Clouds"))
            {
                terrain.GenerateClouds();
            }
        }

        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrain");
        if (showSmooth)
        {
            EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("smoothAmount"));
            if (GUILayout.Button("Smooth"))
                terrain.Smooth();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Heights"))
            terrain.ResetTerrain();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        string fileName = "test";
        if (GUILayout.Button("Save Heights"))
            terrain.SaveHeights(fileName);
        if (GUILayout.Button("Load Heights"))
            terrain.LoadHeights(fileName);
        if (GUILayout.Button("Load Sample Heights"))
            terrain.LoadHeights("sample");

        showHeights = EditorGUILayout.Foldout(showHeights, "Height Map");
        if (showHeights)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int htmSize = (int)(EditorGUIUtility.currentViewWidth - 100);
            GUILayout.Label(hmTexture, GUILayout.Width(htmSize), GUILayout.Height(htmSize));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh", GUILayout.Width(htmSize)))
            {
                float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                for (int y = 0; y < terrain.terrainData.heightmapResolution; y++)
                {
                    for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
                    {
                        hmTexture.SetPixel(x, y, new Color(heightMap[x, y], heightMap[x, y], heightMap[x, y], 1));
                    }
                }
                hmTexture.Apply();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(htmSize)))
            {
                byte[] bytes = hmTexture.EncodeToPNG();
                System.IO.Directory.CreateDirectory(Application.dataPath + "/SavedTextures");
                File.WriteAllBytes(Application.dataPath + "/SavedTextures/" + System.DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH-mm-ss") + ".png", bytes);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
