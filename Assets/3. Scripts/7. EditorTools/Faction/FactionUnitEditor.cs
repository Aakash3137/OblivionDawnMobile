#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FactionUnitEditor : EditorWindow
{
    Vector2 leftScroll, rightScroll;

    UnitProduceStatsSO selectedUnit;
    SerializedObject serializedUnit;

    // Cached properties (IMPORTANT)
    SerializedProperty unitIdentity;
    SerializedProperty unitType;
    SerializedProperty unitPopulationCost;
    SerializedProperty unitPrefab;
    SerializedProperty projectilePrefab;

    SerializedProperty unitVisuals;
    SerializedProperty unitVisionAngles;
    SerializedProperty unitAttackTargets;
    SerializedProperty unitFlyStats;
    SerializedProperty canFly;

    Dictionary<FactionName, List<UnitProduceStatsSO>> factionUnits;

    [MenuItem("Custom Editor/Faction Unit Editor")]
    static void Open()
    {
        GetWindow<FactionUnitEditor>("Faction Units");
    }

    void OnEnable()
    {
        LoadUnits();
    }

    void LoadUnits()
    {
        factionUnits = new Dictionary<FactionName, List<UnitProduceStatsSO>>();

        foreach (FactionName f in System.Enum.GetValues(typeof(FactionName)))
            factionUnits[f] = new List<UnitProduceStatsSO>();

        var units = AssetDatabase.FindAssets("t:UnitProduceStatsSO")
            .Select(g => AssetDatabase.LoadAssetAtPath<UnitProduceStatsSO>(
                AssetDatabase.GUIDToAssetPath(g)));

        foreach (var u in units)
        {
            if (u != null)
                factionUnits[u.unitIdentity.faction].Add(u);
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawLeft();
        DrawRight();
        EditorGUILayout.EndHorizontal();
    }

    // ================= LEFT =================
    void DrawLeft()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(280));
        GUILayout.Label("Factions", EditorStyles.boldLabel);

        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);

        foreach (var kv in factionUnits)
        {
            GUILayout.Label(kv.Key.ToString(), EditorStyles.toolbarButton);

            foreach (var unit in kv.Value)
            {
                if (GUILayout.Button("   " + unit.name,
                    selectedUnit == unit ? EditorStyles.toolbarButton : EditorStyles.miniButton))
                {
                    SelectUnit(unit);
                }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void SelectUnit(UnitProduceStatsSO unit)
    {
        selectedUnit = unit;
        serializedUnit = new SerializedObject(unit);

        // Cache properties ONCE
        unitIdentity = serializedUnit.FindProperty("unitIdentity");
        unitType = serializedUnit.FindProperty("unitType");
        unitPopulationCost = serializedUnit.FindProperty("unitPopulationCost");
        unitPrefab = serializedUnit.FindProperty("unitPrefab");
        projectilePrefab = serializedUnit.FindProperty("projectilePrefab");

        unitVisuals = serializedUnit.FindProperty("unitVisuals");
        unitVisionAngles = serializedUnit.FindProperty("unitVisionAngles");
        unitAttackTargets = serializedUnit.FindProperty("unitAttackTargets");
        unitFlyStats = serializedUnit.FindProperty("unitFlyStats");
        canFly = serializedUnit.FindProperty("canFly");
    }

    // ================= RIGHT =================
    void DrawRight()
    {
        EditorGUILayout.BeginVertical();

        if (selectedUnit == null)
        {
            EditorGUILayout.HelpBox("Select a unit from the left panel", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        serializedUnit.Update();

        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        GUILayout.Label(selectedUnit.name, EditorStyles.largeLabel);
        GUILayout.Space(8);

        DrawProp(unitIdentity);
        DrawProp(unitType);
        DrawProp(unitPopulationCost);
        DrawProp(unitPrefab);
        DrawProp(projectilePrefab);

        GUILayout.Space(10);
        GUILayout.Label("Base Stats", EditorStyles.boldLabel);

        DrawProp(unitVisuals);
        DrawProp(unitVisionAngles);
        DrawProp(unitAttackTargets);
        DrawProp(unitFlyStats);
        DrawProp(canFly);

        EditorGUILayout.EndScrollView();

        if (serializedUnit.ApplyModifiedProperties())
            EditorUtility.SetDirty(selectedUnit);

        EditorGUILayout.EndVertical();
    }

    // ================= SAFE PROPERTY DRAW =================
    void DrawProp(SerializedProperty prop)
    {
        if (prop == null)
        {
            EditorGUILayout.HelpBox("Missing SerializedProperty", MessageType.Error);
            return;
        }

        EditorGUILayout.PropertyField(prop, true);
    }
}
#endif