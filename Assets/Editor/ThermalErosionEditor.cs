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
    void OnEnable()
    {
        factor = serializedObject.FindProperty("TalusFactor");
        talusFactor = serializedObject.FindProperty("Factor");
        iterations = serializedObject.FindProperty("Iterations");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ThermalErosionComponent component = (ThermalErosionComponent)target;

        EditorGUILayout.PropertyField(talusFactor);
        EditorGUILayout.PropertyField(factor);
        EditorGUILayout.PropertyField(iterations);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}