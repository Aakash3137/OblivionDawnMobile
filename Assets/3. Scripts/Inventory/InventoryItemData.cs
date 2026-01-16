using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemId;
    public string itemName;
    public string itemType;
    public FactionName factionType;
    [TextArea] public string description;
    public Canvas itemCanvas;
    public bool IsEquipped = false;

    public InventoryItemData(string id, string UnityType, Canvas _Canvas, string Details, FactionName _FName, bool Status)
    {
        itemId = id;
        itemName = id;
        itemType = UnityType;
        factionType = _FName;
        description = Details;
        itemCanvas = _Canvas;
        IsEquipped = Status;
    }
}