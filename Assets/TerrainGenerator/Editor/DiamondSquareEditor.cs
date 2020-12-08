using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiamondSquareComponent))]
[CanEditMultipleObjects]
public class DiamondSquareEditor : Editor
{
    private SerializedProperty randomGeneration;
    private SerializedProperty seed;
    private SerializedProperty shader;
    private SerializedProperty useGPU;
    private SerializedProperty onlyBestResults;
    private SerializedProperty tries;
    private SerializedProperty minimumFirstValue;

    void OnEnable()
    {
        randomGeneration = serializedObject.FindProperty("randomGeneration");
        seed = serializedObject.FindProperty("seed");
        shader = serializedObject.FindProperty("shader");
        useGPU = serializedObject.FindProperty("useGPU");
        onlyBestResults = serializedObject.FindProperty("onlyBestResults");
        tries = serializedObject.FindProperty("tries");
        minimumFirstValue = serializedObject.FindProperty("minimumFirstValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DiamondSquareComponent component = (DiamondSquareComponent)target;

        EditorGUILayout.PropertyField(randomGeneration);
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(shader);
        EditorGUILayout.PropertyField(useGPU);
        EditorGUILayout.PropertyField(onlyBestResults);
        EditorGUILayout.PropertyField(tries);
        EditorGUILayout.PropertyField(minimumFirstValue);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
