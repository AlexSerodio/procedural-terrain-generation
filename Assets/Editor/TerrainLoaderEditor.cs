using UnityEngine;
using UnityEditor;
using Unity.Components;

[CustomEditor(typeof(TerrainLoaderComponent))]
[CanEditMultipleObjects]
public class TerrainLoaderEditor : Editor
{
    private SerializedProperty texture;
    private SerializedProperty terrain;

    private HeightmapLoader loader = new HeightmapLoader();

    void OnEnable()
    {
        texture = serializedObject.FindProperty("texture");
        terrain = serializedObject.FindProperty("terrain");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TerrainLoaderComponent component = (TerrainLoaderComponent)target;

        EditorGUILayout.PropertyField(texture);
        EditorGUILayout.PropertyField(terrain);
        
        if (GUILayout.Button("Load"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
