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
        if (!lookup.TryGetValue(itemId, out var data))
        {
            Debug.LogError($"Item ID not found: {itemId}");
            return null;
        }

        Item item = Instantiate(data.itemUIPrefab);

        item.SetupItem(
            itemId,
            UnitType,
            data.icon,
            Details,
            IsEquipped,
            _FName,
            detailsWindow,
            _Canvas
        );

        return item;
    }
}
