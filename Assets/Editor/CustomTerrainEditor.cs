using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;

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

    SerializedProperty erosionType;
    SerializedProperty erosionSmoothAmount;

    bool showPerlinNoise = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;
    bool showErosion = false;

    void OnEnable()
    {
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
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

        erosionType = serializedObject.FindProperty("erosionType");
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CustomTerrain terrain = (CustomTerrain)target;

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

        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth Amount"));

            if (GUILayout.Button("Erode"))
                terrain.Erode();
        }

        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrain");
        if (showSmooth)
        {
            EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("smoothAmount"));
            if (GUILayout.Button("Smooth"))
                terrain.Smooth(smoothAmount.intValue);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Heights"))
            terrain.ResetTerrain();

        serializedObject.ApplyModifiedProperties();
    }
}
