using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    public Identity buildingIdentity;
    public ScenarioBuildingType buildingType;
    public Visuals buildingVisuals;
    public BuildCost[] buildingBuildCost;

    public Sprite buildingIcon;

    internal virtual void ValidateBase() { }
    internal virtual void OnValidate()
    {
        ValidateBase();
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