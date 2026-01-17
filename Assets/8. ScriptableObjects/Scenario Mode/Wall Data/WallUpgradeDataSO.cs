using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall UpgradeData SO", menuName = "Upgrade Data/Wall Upgrade Data")]
public class WallUpgradeDataSO : BuildingUpgradeDataSO
{
    public ScenarioDefenseType defenseType;

    internal override void ValidateBase()
    {
        base.ValidateBase();

        buildingType = ScenarioBuildingType.DefenseBuilding;
        defenseType = ScenarioDefenseType.Wall;
    }
}