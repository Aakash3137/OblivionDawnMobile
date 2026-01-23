using UnityEngine;

public class MainBuildingStats : BuildingStats
{
    internal override void Start()
    {
        if (buildingStats is MainBuildingDataSO mainBuildingStats)
        {

        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing MainBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();
    }
}
