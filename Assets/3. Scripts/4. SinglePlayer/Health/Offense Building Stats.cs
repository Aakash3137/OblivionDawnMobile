using UnityEngine;

public class OffenseBuildingStats : BuildingStats
{
    internal override void Start()
    {
        if (buildingStats is OffenseBuildingDataSO offenseBuildingStats)
        {

        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing OffenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();
    }
}
