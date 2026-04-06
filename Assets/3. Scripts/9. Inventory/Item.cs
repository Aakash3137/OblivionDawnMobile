using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] internal FactionName factionType;
    [SerializeField] private string itemType;
    [SerializeField] internal DecCategory _Category;

    [Header("UI")]
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemButton;
    [SerializeField] internal Canvas itemCanvas;
    [SerializeField] internal Slider ItemSlider;

    [Header("State")]
    public bool isEquipped;

    [Header("Game Data")]
    [SerializeField] internal UnitProduceStatsSO unit;
    [SerializeField] internal DefenseBuildingDataSO Defense;
    [SerializeField] internal ResourceBuildingDataSO Resource;

    public InventorySlot CurrentSlot { get; set; }
    internal ItemDetailsWindow itemDetailsWindow;

    private void Awake()
    {
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnClickItem);
    }

    private void OnClickItem()
    {
        // if (itemDetailsWindow == null) return;

        // itemDetailsWindow.ShowItemDetails(
        //     itemName,
        //     itemDescription,
        //     itemImage.sprite,
        //     itemType,
        //     factionType
        // );
    }

    public void SetupItem(string _name, string _type, Sprite _icon, string _details, bool _isEquipped, FactionName _factionType, ItemDetailsWindow detailsWindow, Canvas _Canvas)
    {
        // itemName = _name;
        // itemType = _type;
        // itemImage.sprite = _icon;
        // itemDescription = _details;
        // isEquipped = _isEquipped;
        // factionType = _factionType;
        // itemDetailsWindow = detailsWindow;
        // _Canvas = itemCanvas;
        // if(unit != null)
        // {
        //     if(_Category == DecCategory.Offense)
        //         itemDetailsWindow.unit = unit;
        //     else if(_Category == DecCategory.Defense)
        //          itemDetailsWindow.Defense = Defense;
        //     else if(_Category == DecCategory.Resource)
        //     {
        //         itemDetailsWindow.Resourse = Resource;
        //     }
        // }
    }
}
