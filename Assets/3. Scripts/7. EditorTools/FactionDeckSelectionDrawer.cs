using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FactionDeckSelection))]
public class FactionDeckSelectionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 2f; // Faction name
        
        var unitsProp = property.FindPropertyRelative("unitSelections");
        var defenseProp = property.FindPropertyRelative("defenseBuildingSelections");
        
        if (property.isExpanded)
        {
            if (unitsProp.arraySize > 0)
            {
                height += EditorGUIUtility.singleLineHeight + 4f; // Units header
                height += unitsProp.arraySize * (EditorGUIUtility.singleLineHeight + 2f);
            }
            
            if (defenseProp.arraySize > 0)
            {
                height += EditorGUIUtility.singleLineHeight + 8f; // Defense header with spacing
                height += defenseProp.arraySize * (EditorGUIUtility.singleLineHeight + 2f);
            }
        }
        
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var factionProp = property.FindPropertyRelative("faction");
        var unitsProp = property.FindPropertyRelative("unitSelections");
        var defenseProp = property.FindPropertyRelative("defenseBuildingSelections");

        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, factionProp.enumDisplayNames[factionProp.enumValueIndex], true);

        if (!property.isExpanded) return;

        float yOffset = foldoutRect.yMax + 2f;

        // Units Section
        if (unitsProp.arraySize > 0)
        {
            DrawColumnHeaders(new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight), "Unit Name");
            yOffset += EditorGUIUtility.singleLineHeight + 2f;

            for (int i = 0; i < unitsProp.arraySize; i++)
            {
                Rect elementRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(elementRect, unitsProp.GetArrayElementAtIndex(i), GUIContent.none);
                yOffset += EditorGUIUtility.singleLineHeight + 2f;
            }
        }

        // Defense Section
        if (defenseProp.arraySize > 0)
        {
            yOffset += 4f; // Extra spacing
            DrawColumnHeaders(new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight), "Defense Name");
            yOffset += EditorGUIUtility.singleLineHeight + 2f;

            for (int i = 0; i < defenseProp.arraySize; i++)
            {
                Rect elementRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(elementRect, defenseProp.GetArrayElementAtIndex(i), GUIContent.none);
                yOffset += EditorGUIUtility.singleLineHeight + 2f;
            }
        }
    }

    private void DrawColumnHeaders(Rect position, string firstColumnName)
    {
        float toggleWidth = position.width - 170f;
        
        Rect nameRect = new Rect(position.x + 15f, position.y, toggleWidth - 15f, position.height);
        Rect amountRect = new Rect(position.x + toggleWidth + 5f, position.y, 50f, position.height);
        Rect levelRect = new Rect(position.x + toggleWidth + 60f, position.y, 50f, position.height);
        Rect weightRect = new Rect(position.x + toggleWidth + 115f, position.y, 50f, position.height);

        var style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 10;

        EditorGUI.LabelField(nameRect, firstColumnName, style);
        EditorGUI.LabelField(amountRect, "Amt", style);
        EditorGUI.LabelField(levelRect, "Lvl", style);
        EditorGUI.LabelField(weightRect, "Wgt", style);
    }
}
#endif
