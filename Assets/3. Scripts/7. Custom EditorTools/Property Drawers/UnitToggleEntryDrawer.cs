using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UnitToggleEntry))]
public class UnitToggleEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var unitProp = property.FindPropertyRelative("unit");
        var selectedProp = property.FindPropertyRelative("selected");
        var amountProp = property.FindPropertyRelative("amount");
        var spawnLevelProp = property.FindPropertyRelative("UnitsSpawnLevel");
        var weightProp = property.FindPropertyRelative("weight");

        if (unitProp.objectReferenceValue == null)
        {
            EditorGUI.LabelField(position, "Missing Unit");
            return;
        }

        var unit = unitProp.objectReferenceValue as UnitProduceStatsSO;
        string displayName = unit != null ? unit.unitIdentity.name : "Unnamed Unit";

        float toggleWidth = position.width - 170f;

        Rect toggleRect = new Rect(position.x, position.y, toggleWidth, position.height);
        Rect amountRect = new Rect(position.x + toggleWidth + 5f, position.y, 50f, position.height);
        Rect levelRect = new Rect(position.x + toggleWidth + 60f, position.y, 50f, position.height);
        Rect weightRect = new Rect(position.x + toggleWidth + 115f, position.y, 50f, position.height);

        selectedProp.boolValue =
            EditorGUI.ToggleLeft(toggleRect, displayName, selectedProp.boolValue);

        EditorGUI.BeginDisabledGroup(!selectedProp.boolValue);
        EditorGUI.PropertyField(amountRect, amountProp, GUIContent.none);
        EditorGUI.PropertyField(levelRect, spawnLevelProp, GUIContent.none);
        EditorGUI.PropertyField(weightRect, weightProp, GUIContent.none);
        EditorGUI.EndDisabledGroup();
    }
}
#endif