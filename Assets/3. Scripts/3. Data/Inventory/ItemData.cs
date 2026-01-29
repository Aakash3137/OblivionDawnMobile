using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemId;

    [Header("Display")]
    public string itemName;
    public string itemType;
    public FactionName factionType;
    [TextArea] public string description;
    public Canvas ItemCanvas;
    public Sprite icon;

    [Header("UI")]
    public Item itemUIPrefab;

    [Header("Game Data")] 
    [SerializeField]internal UnitProduceStatsSO ItemSo;
}
