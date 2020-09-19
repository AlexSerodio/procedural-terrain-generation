using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneralTerrainComponent))]
[CanEditMultipleObjects]
public class GeneralTerrainEditor : Editor
{
    private SerializedProperty smoothAmount;

    void OnEnable()
    {
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        // Tools.current = Tool.None;  // hide gizmos
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GeneralTerrainComponent component = (GeneralTerrainComponent)target;
        
        EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("smoothAmount"));
        if (GUILayout.Button("Smooth"))
            component.SmoothTerrain(smoothAmount.intValue);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Heights"))
            component.ResetTerrain();

        serializedObject.ApplyModifiedProperties();
    }
}
