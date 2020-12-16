using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeightmapLoaderComponent))]
[CanEditMultipleObjects]
public class HeightmapLoaderEditor : Editor
{
    private SerializedProperty texture;

    void OnEnable()
    {
        texture = serializedObject.FindProperty("texture");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HeightmapLoaderComponent component = (HeightmapLoaderComponent)target;

        EditorGUILayout.PropertyField(texture);
        
        if (GUILayout.Button("Import"))
            component.UpdateComponent();

        if (GUILayout.Button("Export"))
            component.export();

        serializedObject.ApplyModifiedProperties();
    }
}
