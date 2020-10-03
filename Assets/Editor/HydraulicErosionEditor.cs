using Unity.Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HydraulicErosionComponent))]
[CanEditMultipleObjects]
public class HydraulicErosionEditor : Editor
{
    private SerializedProperty rainFactor;
    private SerializedProperty solubility;
    private SerializedProperty evaporationFactor;
    private SerializedProperty sedimentCapacity;
    private SerializedProperty iterations;

    private SerializedProperty diegoli;
    
    void OnEnable()
    {
        rainFactor = serializedObject.FindProperty("rainFactor");
        solubility = serializedObject.FindProperty("solubility");
        evaporationFactor = serializedObject.FindProperty("evaporationFactor");
        sedimentCapacity = serializedObject.FindProperty("sedimentCapacity");
        iterations = serializedObject.FindProperty("iterations");
        diegoli = serializedObject.FindProperty("diegoli");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HydraulicErosionComponent component = (HydraulicErosionComponent)target;

        EditorGUILayout.PropertyField(rainFactor);
        EditorGUILayout.PropertyField(solubility);
        EditorGUILayout.PropertyField(evaporationFactor);
        EditorGUILayout.PropertyField(sedimentCapacity);
        EditorGUILayout.PropertyField(iterations);
        EditorGUILayout.PropertyField(diegoli);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
