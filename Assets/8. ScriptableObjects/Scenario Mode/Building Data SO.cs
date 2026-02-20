using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    public Identity buildingIdentity;
    public ScenarioBuildingType buildingType;
    public Visuals buildingVisuals;
    public BuildCost[] buildingBuildCost;

    public Sprite buildingIcon;

    internal virtual void ValidateBase()
    {
        var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

        if (buildingBuildCost == null || buildingBuildCost.Length != enumValues.Length)
        {
            buildingBuildCost = new BuildCost[enumValues.Length];
        }

        for (int j = 0; j < buildingBuildCost.Length; j++)
        {
            buildingBuildCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
        }
    }

    internal virtual void OnValidate()
    {
        ValidateBase();
    }

    [Button]
    public void SetBuildingCost(float increasePercent)
    {
        increasePercent = increasePercent / 100f;
        float discount = 1f + increasePercent;

        BuildCost[] baseCost = buildingBuildCost;

        for (int i = 0; i < buildingBuildCost.Length; i++)
        {
            buildingBuildCost[i].resourceAmount = Mathf.RoundToInt(baseCost[i].resourceAmount * discount);
        }
    }
}

[Serializable]
public struct Identity
{
    public string name;
    public int spawnLevel;
    public FactionName faction;
    public bool isUnique;
    public int priority;
}

[Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    [Space(30)]
    public BasicStats buildingBasicStats;
    public float buildingBuildTime;
}