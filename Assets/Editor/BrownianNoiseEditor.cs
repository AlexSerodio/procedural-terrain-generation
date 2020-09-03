using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrownianNoiseComponent))]
[CanEditMultipleObjects]
public class BrownianNoiseEditor : Editor
{
    private SerializedProperty perlinOffsetX;
    private SerializedProperty perlinOffsetY;
    private SerializedProperty perlinXScale;
    private SerializedProperty perlinYScale;
    private SerializedProperty perlinOctaves;
    private SerializedProperty perlinPersistance;
    private SerializedProperty perlinHeightScale;

    void OnEnable()
    {
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        BrownianNoiseComponent component = (BrownianNoiseComponent)target;

        EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
        EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
        EditorGUILayout.Slider(perlinXScale, 0, 0.1f, new GUIContent("X Scale"));
        EditorGUILayout.Slider(perlinYScale, 0, 0.1f, new GUIContent("Y Scale"));
        EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
        EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
        EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}