using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoronoiComponent))]
[CanEditMultipleObjects]
public class VoronoiEditor : Editor
{
    private SerializedProperty fallOff;
    private SerializedProperty dropOff;
    private SerializedProperty minHeight;
    private SerializedProperty maxHeight;
    private SerializedProperty peaksAmount;
    private SerializedProperty type;

    void OnEnable()
    {
        dropOff = serializedObject.FindProperty("dropOff");
        fallOff = serializedObject.FindProperty("fallOff");
        minHeight = serializedObject.FindProperty("minHeight");
        maxHeight = serializedObject.FindProperty("maxHeight");
        peaksAmount = serializedObject.FindProperty("peaksAmount");
        type = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        VoronoiComponent component = (VoronoiComponent)target;

        EditorGUILayout.Slider(fallOff, 0, 10, new GUIContent("Falloff"));
        EditorGUILayout.Slider(dropOff, 0, 10, new GUIContent("Dropoff"));
        EditorGUILayout.Slider(minHeight, 0, 1, new GUIContent("Min Height"));
        EditorGUILayout.Slider(maxHeight, 0, 1, new GUIContent("Max Height"));
        EditorGUILayout.IntSlider(peaksAmount, 1, 10, new GUIContent("Peak Count"));
        EditorGUILayout.PropertyField(type);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}