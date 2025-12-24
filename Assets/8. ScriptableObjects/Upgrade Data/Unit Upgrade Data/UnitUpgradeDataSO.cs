using UnityEngine;

[CreateAssetMenu(fileName = "Unit Upgrade Data", menuName = "Game/Unit Upgrade Data")]
public class UnitUpgradeDataSO : ScriptableObject
{

    [Header("Upgrade data is stored in array 0 represents Base Level. 1-n can represent level of upgrade")]
    public string unitType;
    public UnitUpgradeData[] unitLevelData;
}