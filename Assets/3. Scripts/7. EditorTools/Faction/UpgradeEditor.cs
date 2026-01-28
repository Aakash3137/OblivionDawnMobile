using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class UpgradeEditor : EditorWindow
{
    // ================== EDITOR STATE ==================
    FactionName selectedFaction;
    ScenarioUnitType selectedUnitType;

    UnitProduceStatsSO unit;
    Vector2 scroll;

    int maxLevel = 20;
    int unlockedLevel = 0;

    // ================== MENU ==================
    [MenuItem("Scenario Tools/Upgrade Authoring")]
    static void Open() => GetWindow<UpgradeEditor>("Upgrade Authoring");

    // ================== GUI ==================
    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawSelectionPanel();
        EditorGUILayout.Space(8);

        if (unit == null)
        {
            EditorGUILayout.HelpBox("Select Faction and Unit Type", MessageType.Info);
            EditorGUILayout.EndScrollView();
            return;
        }

        DrawEditorControls();
        EditorGUILayout.Space(6);

        DrawTable();
        EditorGUILayout.Space(8);

        DrawReset();

        EditorGUILayout.EndScrollView();
    }

    // ================== SELECTION ==================
    void DrawSelectionPanel()
    {
        EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

        selectedFaction =
            (FactionName)EditorGUILayout.EnumPopup("Faction", selectedFaction);

        selectedUnitType =
            (ScenarioUnitType)EditorGUILayout.EnumPopup("Unit Type", selectedUnitType);

        unit = FindUnit(selectedFaction, selectedUnitType);
    }

    UnitProduceStatsSO FindUnit(FactionName faction, ScenarioUnitType type)
    {
        var guids = AssetDatabase.FindAssets("t:UnitProduceStatsSO");

        foreach (var g in guids)
        {
            var so = AssetDatabase.LoadAssetAtPath<UnitProduceStatsSO>(
                AssetDatabase.GUIDToAssetPath(g));

            if (so == null) continue;

            if (so.unitIdentity.faction == faction &&
                so.unitType == type)
                return so;
        }

        return null;
    }

    // ================== CONTROLS ==================
    void DrawEditorControls()
    {
        EditorGUILayout.BeginVertical("box");

        maxLevel = EditorGUILayout.IntSlider("Max Level", maxLevel, 1, 50);

        unlockedLevel = unit.unitUpgradeData.Length - 1;
        EditorGUILayout.LabelField("Unlocked Level", unlockedLevel.ToString());

        EditorGUILayout.EndVertical();
    }

    // ================== TABLE ==================
    void DrawTable()
    {
        var data = unit.unitUpgradeData;
        if (data == null || data.Length == 0)
            return;

        DrawHeader();

        for (int i = 0; i < data.Length; i++)
        {
            DrawRow(data[i], i);
        }
    }

    void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        Label("Lvl", 30);
        Label("Cost", 70);
        Label("Build", 60);
        Label("HP", 60);
        Label("DMG", 60);
        Label("ARM", 60);
        Label("FR", 60);
        Label("SPD", 60);
        Label("DET", 60);
        Label("RNG", 60);
        GUILayout.Space(70);
        EditorGUILayout.EndHorizontal();
    }

    void DrawRow(UnitUpgradeData d, int index)
    {
        EditorGUILayout.BeginHorizontal("box");

        Label(d.unitLevel.ToString(), 30);

        float cost = CostFormula(d.unitLevel);
        Label(cost.ToString("0.0"), 70);

        RO(d.unitBuildTime);
        RO(d.unitBasicStats.maxHealth);
        RO(d.unitAttackStats.damage);
        RO(d.unitBasicStats.armor);
        RO(d.unitAttackStats.fireRate);
        RO(d.unitMobilityStats.moveSpeed);
        RO(d.unitRangeStats.detectionRange);
        RO(d.unitRangeStats.attackRange);

        GUILayout.Space(10);

        GUI.enabled = index == unit.unitUpgradeData.Length - 1 &&
                      index < maxLevel;

        if (GUILayout.Button("Upgrade", GUILayout.Width(60)))
            UpgradeNext();

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }

    // ================== ACTIONS ==================
    void UpgradeNext()
    {
        int next = unit.unitUpgradeData.Length;
        if (next > maxLevel) return;

        Undo.RecordObject(unit, "Upgrade Level");

        var prev = unit.unitUpgradeData[next - 1];

        Array.Resize(ref unit.unitUpgradeData, next + 1);

        var cur = new UnitUpgradeData();
        cur.unitLevel = next;

        ApplyFormula(prev, ref cur);

        unit.unitUpgradeData[next] = cur;
        unit.unitIdentity.spawnLevel = next;

        EditorUtility.SetDirty(unit);
    }

    void DrawReset()
    {
        EditorGUILayout.Space(10);

        if (GUILayout.Button("RESET TO BASE LEVEL", GUILayout.Height(30)))
        {
            if (!EditorUtility.DisplayDialog(
                "Reset Unit",
                "This will delete all upgraded levels.",
                "Reset", "Cancel"))
                return;

            Undo.RecordObject(unit, "Reset Levels");

            var baseLevel = unit.unitUpgradeData[0];
            baseLevel.unitLevel = 0;

            unit.unitUpgradeData = new[] { baseLevel };
            unit.unitIdentity.spawnLevel = 0;

            EditorUtility.SetDirty(unit);
        }
    }

    // ================== FORMULAS ==================
    void ApplyFormula(UnitUpgradeData prev, ref UnitUpgradeData cur)
    {
        const float P = 0.45f;

        cur.unitBuildTime =
            Scale(prev.unitBuildTime, P, false);

        cur.unitBasicStats = new BasicStats
        {
            maxHealth = Scale(prev.unitBasicStats.maxHealth, P),
            armor = Scale(prev.unitBasicStats.armor, P)
        };

        cur.unitAttackStats = new AttackStats
        {
            damage = Scale(prev.unitAttackStats.damage, P),
            fireRate = Scale(prev.unitAttackStats.fireRate, P)
        };

        cur.unitMobilityStats = new MobilityStats
        {
            moveSpeed = Scale(prev.unitMobilityStats.moveSpeed, P)
        };

        cur.unitRangeStats = new RangeStats
        {
            detectionRange = Scale(prev.unitRangeStats.detectionRange, P),
            attackRange = Scale(prev.unitRangeStats.attackRange, P)
        };
    }

    float Scale(float v, float p, bool inc = true)
    {
        float r = inc ? v * (1f + p) : v * (1f - p);
        return (float)Math.Round(r, 1);
    }

    float CostFormula(int n)
    {
        return 10f * n * n + 10f * n + 20f;
    }

    // ================== UI HELPERS ==================
    void Label(string t, int w) =>
        GUILayout.Label(t, GUILayout.Width(w));

    void RO(float v) =>
        EditorGUILayout.FloatField(v, GUILayout.Width(60));
}



public static class UpgradeFormula
{
    public const float P = 0.45f;

    public static float Calc(
        float baseValue,
        int level,
        int maxLevel,
        bool increase)
    {
        float t = (float)level / maxLevel;
        float v = increase
            ? baseValue * (1f + P * t)
            : baseValue * (1f - P * t);

        return UpgradeEditorConfig.Round1(v);
    }
}

public static class UpgradeLevelGenerator
{
    public static UnitUpgradeData[] Generate(UnitUpgradeData baseLevel, int maxLevel)
    {
        var levels = new UnitUpgradeData[maxLevel + 1];

        for (int i = 0; i <= maxLevel; i++)
        {
            levels[i] = new UnitUpgradeData
            {
                unitLevel = i,

                unitBuildTime = UpgradeFormula.Calc(
                    baseLevel.unitBuildTime, i, maxLevel, false),

                unitBasicStats = new BasicStats
                {
                    maxHealth = UpgradeFormula.Calc(
                        baseLevel.unitBasicStats.maxHealth, i, maxLevel, true),

                    armor = UpgradeFormula.Calc(
                        baseLevel.unitBasicStats.armor, i, maxLevel, true)
                },

                unitAttackStats = new AttackStats
                {
                    damage = UpgradeFormula.Calc(
                        baseLevel.unitAttackStats.damage, i, maxLevel, true),

                    fireRate = UpgradeFormula.Calc(
                        baseLevel.unitAttackStats.fireRate, i, maxLevel, true)
                },

                unitMobilityStats = new MobilityStats
                {
                    moveSpeed = UpgradeFormula.Calc(
                        baseLevel.unitMobilityStats.moveSpeed, i, maxLevel, true)
                },

                unitRangeStats = new RangeStats
                {
                    detectionRange = UpgradeFormula.Calc(
                        baseLevel.unitRangeStats.detectionRange, i, maxLevel, true),

                    attackRange = UpgradeFormula.Calc(
                        baseLevel.unitRangeStats.attackRange, i, maxLevel, true)
                }
            };
        }

        return levels;
    }
}

static class UpgradeEditorConfig
{
    public const int DEFAULT_MAX_LEVEL = 20;

    // Cost formula: 10n² + 10n + 20
    public static int Cost(int level)
        => 10 * level * level + 10 * level + 20;

    public static float Round1(float v)
        => Mathf.Round(v * 10f) / 10f;
}
