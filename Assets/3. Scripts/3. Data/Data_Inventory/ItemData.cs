using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemId;

    [Header("Display")]
    public string itemName;
    public string itemType;
    public FactionName factionType;
    public DecCategory _Catgory;
    [TextArea] public string description;
    public Canvas ItemCanvas;
    public Sprite icon;

    [Header("UI")]
    public Item itemUIPrefab;

    [Header("Game Data = Selected Type Wise give reference")] 
    [SerializeField]internal UnitProduceStatsSO ItemSo;
    [SerializeField] internal DefenseBuildingDataSO DefenseSo;
    [SerializeField] internal ResourceBuildingDataSO BuildingSO;
}
