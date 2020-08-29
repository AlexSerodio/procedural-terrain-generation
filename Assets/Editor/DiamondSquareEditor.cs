using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiamondSquareComponent))]
[CanEditMultipleObjects]
public class MidpointDisplacementEditor : Editor
{
    private SerializedProperty resolution;
    private SerializedProperty height;

    void OnEnable()
    {
        resolution = serializedObject.FindProperty("resolution");
        height = serializedObject.FindProperty("height");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DiamondSquareComponent component = (DiamondSquareComponent)target;

        EditorGUILayout.PropertyField(resolution);
        EditorGUILayout.PropertyField(height);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}