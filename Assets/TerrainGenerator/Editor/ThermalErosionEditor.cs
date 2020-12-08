using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThermalErosionComponent))]
[CanEditMultipleObjects]
public class ThermalErosionEditor : Editor
{
    private SerializedProperty talusFactor;
    private SerializedProperty factor;
    private SerializedProperty iterations;
    private SerializedProperty shader;
    private SerializedProperty useGPU;
    
    void OnEnable()
    {
        factor = serializedObject.FindProperty("talusFactor");
        talusFactor = serializedObject.FindProperty("factor");
        iterations = serializedObject.FindProperty("iterations");
        shader = serializedObject.FindProperty("shader");
        useGPU = serializedObject.FindProperty("useGPU");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ThermalErosionComponent component = (ThermalErosionComponent)target;

        EditorGUILayout.PropertyField(talusFactor);
        EditorGUILayout.PropertyField(factor);
        EditorGUILayout.PropertyField(iterations);
        EditorGUILayout.PropertyField(shader);
        EditorGUILayout.PropertyField(useGPU);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
