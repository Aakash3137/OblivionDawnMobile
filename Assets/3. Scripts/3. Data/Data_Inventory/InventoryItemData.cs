using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemId;
    public string itemName;
    public string itemType;
    public FactionName factionType;
    public DecCategory _Dec;
    public UnitProduceStatsSO Units;
    public DefenseBuildingDataSO Defenses;
    public ResourceBuildingDataSO Resources;
    [TextArea] public string description;
    public Canvas itemCanvas;
    public bool IsEquipped = false;

    public InventoryItemData(string id, string UnityType, Canvas _Canvas, string Details, FactionName _FName, bool Status, DecCategory Dec, DecSelector DecData)
    {
        itemId = id;
        itemName = id;
        itemType = UnityType;
        factionType = _FName;
        description = Details;
        itemCanvas = _Canvas;
        IsEquipped = Status;
        _Dec = Dec;
        
        foreach(var item in DecData.UnitCards)
            {
                if(id == item.unitIdentity.name)
                {
                    Units = item;
                }
            }

        foreach(var item in DecData.DefenseCards)
            {
                if(id == item.buildingIdentity.name)
                {
                    Defenses = item;
                }
            }

        foreach (var item in DecData.ResourceCards)
        {
            if(id == item.buildingIdentity.name)
                {
                    Resources = item;
                }
        }
    }
}