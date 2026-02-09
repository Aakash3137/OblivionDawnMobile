using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionEntityDetails : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] internal TMP_Text EntityName;
    [SerializeField] internal TMP_Text EntityLevel;
    [SerializeField] internal Image Icon;
    [SerializeField] internal Image OtherIcon;

    // ================================
    // UNIT
    // ================================
    public void SetData(UnitProduceStatsSO unit)
    {
        if (unit == null) return;

        EntityName.text = GetName(unit.unitIdentity.name);
        EntityLevel.text = $"{unit.unitUpgradeData.Length}";

        Icon.sprite = unit.UnitIcon;

        if (OtherIcon != null)
            OtherIcon.sprite = unit.UnitIcon;
    }

    // ================================
    // DEFENSE
    // ================================
    public void SetData(DefenseBuildingDataSO defense)
    {
        if (defense == null) return;

        EntityName.text = GetName(defense.buildingIdentity.name);
        EntityLevel.text = $"{defense.defenseBuildingUpgradeData.Length}";

        Icon.sprite = defense.buildingIcon;

        if (OtherIcon != null)
            OtherIcon.sprite = defense.buildingIcon;
    }

    // ================================
    // RESOURCE
    // ================================
    public void SetData(ResourceBuildingDataSO resource)
    {
        if (resource == null) return;

        EntityName.text = GetName(resource.buildingIdentity.name);
        EntityLevel.text = $"{resource.resourceBuildingUpgradeData.Length}";

        Icon.sprite = resource.buildingIcon;

        if (OtherIcon != null)
            OtherIcon.sprite = resource.buildingIcon; 
    }

    private string GetName(string name)
    {
        string first = name.Split(' ')[0];
        return first;

    }
}
