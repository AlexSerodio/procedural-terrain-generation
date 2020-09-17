using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiamondSquareComponent))]
[CanEditMultipleObjects]
public class DiamondSquareEditor : Editor
{
    private SerializedProperty seed;
    private SerializedProperty shader;
    private SerializedProperty useGPU;

    void OnEnable()
    {
        seed = serializedObject.FindProperty("seed");
        shader = serializedObject.FindProperty("shader");
        useGPU = serializedObject.FindProperty("useGPU");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DiamondSquareComponent component = (DiamondSquareComponent)target;

        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(shader);
        EditorGUILayout.PropertyField(useGPU);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
