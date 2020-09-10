using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiamondSquareComponent))]
[CanEditMultipleObjects]
public class DiamondSquareEditor : Editor
{
    private SerializedProperty resolution;
    private SerializedProperty shader;
    private SerializedProperty useGPU;

    void OnEnable()
    {
        resolution = serializedObject.FindProperty("resolution");
        shader = serializedObject.FindProperty("shader");
        useGPU = serializedObject.FindProperty("useGPU");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DiamondSquareComponent component = (DiamondSquareComponent)target;

        EditorGUILayout.PropertyField(resolution);
        EditorGUILayout.PropertyField(shader);
        EditorGUILayout.PropertyField(useGPU);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
