using UnityEditor;

[CustomPropertyDrawer(typeof(DontShowIfAttribute), true)]
public class DontShowIfAttributeDrawer : ShowIfAttributeDrawer
{
    protected override bool MeetsConditions(SerializedProperty property) => !base.MeetsConditions(property);
}
