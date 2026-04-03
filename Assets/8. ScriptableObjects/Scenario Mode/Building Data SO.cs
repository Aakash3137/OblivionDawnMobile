using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    public GameBuildingName gameBuildingName;
    public Identity buildingIdentity;
    public ScenarioBuildingType buildingType;
    public Visuals buildingVisuals;
    public BuildCost[] buildingBuildCost;
    public Sprite buildingIcon;
    public CardDetails cardDetails;

    [Space(20)]
    public bool hasUpkeep;
    [ShowIf(nameof(hasUpkeep))]
    public BuildCost[] upKeepCost;
    [field: SerializeField, Space(10), ShowIf(nameof(hasUpkeep))]
    public float upKeepTime { get; private set; }

    internal virtual void ValidateBase()
    {
        var enumValues = ScenarioDataTypes._resourceEnumValues;
        int targetLength = enumValues.Length;

        buildingBuildCost = BuildCostUtils.ResizePreservingData(buildingBuildCost, targetLength);

        for (int j = 0; j < buildingBuildCost.Length; j++)
            buildingBuildCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);

        if (!hasUpkeep)
            return;

        upKeepCost = BuildCostUtils.ResizePreservingData(upKeepCost, targetLength);

        for (int j = 0; j < upKeepCost.Length; j++)
            upKeepCost[j].resourceType = enumValues[j];

        if (cardDetails.upgradeCostMultiplier < 0)
            cardDetails.upgradeCostMultiplier = 1f;

        if (cardDetails.fragmentCostMultiplier < 0)
            cardDetails.fragmentCostMultiplier = 1f;
    }

    internal virtual void OnValidate()
    {
        ValidateBase();
    }

    [Button]
    public void SetBuildingCost(float increasePercent)
    {
        float discount = 1f + (increasePercent / 100f);

        for (int i = 0; i < buildingBuildCost.Length; i++)
        {
            if (buildingBuildCost[i].resourceAmount != 0)
            {
                var amt = Mathf.RoundToInt(buildingBuildCost[i].resourceAmount * discount);
                buildingBuildCost[i].resourceAmount = Mathf.Max(amt, 1);
            }
        }
    }

    [ShowIf(nameof(hasUpkeep)), Button]
    public void SetUpKeepCost(float increasePercent)
    {
        float discount = 1f + (increasePercent / 100f);

        for (int i = 0; i < upKeepCost.Length; i++)
        {
            if (upKeepCost[i].resourceAmount != 0)
            {
                var amt = Mathf.RoundToInt(upKeepCost[i].resourceAmount * discount);
                upKeepCost[i].resourceAmount = Mathf.Max(amt, 1);
            }
        }
    }
    [Button]
    public void GenerateLevels()
    {
        StatUpgrade.GenerateUpgradeData(this);
    }
}
