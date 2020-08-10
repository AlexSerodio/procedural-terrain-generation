using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MidpointDisplacementComponent))]
[CanEditMultipleObjects]
public class MidpointDisplacementEditor : Editor
{
    private SerializedProperty heightDampenerPower;
    private SerializedProperty heightMax;
    private SerializedProperty heightMin;
    private SerializedProperty roughness;

    void OnEnable()
    {
        heightDampenerPower = serializedObject.FindProperty("HeightDampenerPower");
        heightMax = serializedObject.FindProperty("HeightMax");
        heightMin = serializedObject.FindProperty("HeightMin");
        roughness = serializedObject.FindProperty("Roughness");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MidpointDisplacementComponent component = (MidpointDisplacementComponent)target;

        EditorGUILayout.PropertyField(heightDampenerPower);
        EditorGUILayout.PropertyField(heightMax);
        EditorGUILayout.PropertyField(heightMin);
        EditorGUILayout.PropertyField(roughness);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}