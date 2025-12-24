using UnityEngine;

[CreateAssetMenu(fileName = "Building Upgrade Data", menuName = "Game/Building Upgrade Data")]
public class BuildingUpgradeDataSO : ScriptableObject
{

    [Header("Upgrade data is stored in array 0 represents Base Level. 1-n can represent level of upgrade")]
    public string buildingType;    
    public BuildingUpgradeData[] buildingLevelData;
}