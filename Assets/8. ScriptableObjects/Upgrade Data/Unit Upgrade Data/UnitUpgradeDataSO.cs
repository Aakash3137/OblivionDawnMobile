using Unity.Android.Gradle.Manifest;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Upgrade Data", menuName = "Upgrade Data/Unit Upgrade Data")]
public class UnitUpgradeDataSO : ScriptableObject
{

    [Header("Upgrade data is stored in array 0 represents Base Level. 1-n can represent level of upgrade")]
    public string unitName;
    public UnitType unitType;
    public FactionName unitFactionName;

    [Tooltip("Level 0 = base Unit")]
    public UnitUpgradeData[] unitLevelData;

    private void OnValidate()
    {
        if (unitLevelData == null) return;
        for (int i = 0; i < unitLevelData.Length; i++)
        {
            unitLevelData[i].unitLevel = i;
        }
    }
}