using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DefenseBuildingToggleEntry))]
public class DefenseBuildingToggleEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var buildingProp = property.FindPropertyRelative("building");
        var selectedProp = property.FindPropertyRelative("selected");
        var amountProp = property.FindPropertyRelative("amount");

        if (buildingProp.objectReferenceValue == null)
        {
            EditorGUI.LabelField(position, "Missing Building");
            return;
        }

        var building = buildingProp.objectReferenceValue as DefenseBuildingDataSO;
        string displayName = building != null ? building.buildingIdentity.name : "Unnamed Building";

        float toggleWidth = position.width - 60f;

        Rect toggleRect = new Rect(position.x, position.y, toggleWidth, position.height);
        Rect amountRect = new Rect(position.x + toggleWidth + 5f, position.y, 50f, position.height);

        selectedProp.boolValue =
            EditorGUI.ToggleLeft(toggleRect, displayName, selectedProp.boolValue);

        EditorGUI.BeginDisabledGroup(!selectedProp.boolValue);
        EditorGUI.PropertyField(amountRect, amountProp, GUIContent.none);
        EditorGUI.EndDisabledGroup();
    }
}