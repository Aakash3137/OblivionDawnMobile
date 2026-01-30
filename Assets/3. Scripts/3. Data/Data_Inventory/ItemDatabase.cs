using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> items;

    private Dictionary<string, ItemData> lookup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        lookup = new Dictionary<string, ItemData>();
        foreach (var item in items)
        {
            if (!lookup.ContainsKey(item.itemId))
                lookup.Add(item.itemId, item);
        }
    }

    public Item CreateItemUI(string itemId, string UnitType, Canvas _Canvas, string Details, FactionName _FName, ItemDetailsWindow detailsWindow, bool IsEquipped)
    {
        GameDebug.LogWarning($"Creating Item UI for Item ID: {itemId} UnitType: {UnitType}   Details: {Details} Faction: {_FName} IsEquipped: {IsEquipped} ItemDatabase Count: {lookup.Count}");
        if (!lookup.TryGetValue(itemId, out var data))
        {
            Debug.LogError($"Item ID not found: {data.name}");
            return null;
        }

        Item item = Instantiate(data.itemUIPrefab);
        item.unit = data.ItemSo;

        item.SetupItem(
            data.itemName,
            data.itemType,
            data.icon,
            Details,
            IsEquipped,
            _FName,
            detailsWindow,
            _Canvas,
            data.ItemSo
        );

        return item;
    }
}
