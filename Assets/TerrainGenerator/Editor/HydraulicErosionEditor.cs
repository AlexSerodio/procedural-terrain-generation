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
    private SerializedProperty iterations;
    private SerializedProperty pourAndDissolveShader;
    private SerializedProperty waterFlowShader;
    private SerializedProperty drainWaterShader;
    private SerializedProperty useGPU;
    
    void OnEnable()
    {
        rainFactor = serializedObject.FindProperty("rainFactor");
        solubility = serializedObject.FindProperty("solubility");
        evaporationFactor = serializedObject.FindProperty("evaporationFactor");
        iterations = serializedObject.FindProperty("iterations");
        pourAndDissolveShader = serializedObject.FindProperty("pourAndDissolveShader");
        waterFlowShader = serializedObject.FindProperty("waterFlowShader");
        drainWaterShader = serializedObject.FindProperty("drainWaterShader");
        useGPU = serializedObject.FindProperty("useGPU");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HydraulicErosionComponent component = (HydraulicErosionComponent)target;

        EditorGUILayout.PropertyField(rainFactor);
        EditorGUILayout.PropertyField(solubility);
        EditorGUILayout.PropertyField(evaporationFactor);
        EditorGUILayout.PropertyField(iterations);
        EditorGUILayout.PropertyField(pourAndDissolveShader);
        EditorGUILayout.PropertyField(waterFlowShader);
        EditorGUILayout.PropertyField(drainWaterShader);
        EditorGUILayout.PropertyField(useGPU);
        
        if (GUILayout.Button("Apply"))
            component.UpdateComponent();

        serializedObject.ApplyModifiedProperties();
    }
}
