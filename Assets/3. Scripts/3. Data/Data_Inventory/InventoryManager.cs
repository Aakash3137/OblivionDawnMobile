using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // ==================================================
    // SLOT SETUP
    // ==================================================

    [Header("Equipped Slots (Static)")]
    [SerializeField] private Transform equippedSlotsParent;
    [SerializeField] private InventorySlot equippedSlotPrefab;
    [SerializeField] private int equippedSlotCount = 4;

    [Header("Unequipped Slots (Dynamic)")]
    [SerializeField] private Transform unequippedSlotsParent;
    [SerializeField] private InventorySlot unequippedSlotPrefab;

    // ==================================================
    // DATA (SAVE TO PLAYFAB)
    // ==================================================

    [Header("Game Data")]
    [SerializeField] private DecSelectionData _SelectionData;

    [Header("Inventory Data")]
    [SerializeField] internal List<InventoryItemData> equippedData = new();
    [SerializeField] internal List<InventoryItemData> unequippedData = new();

    // ==================================================
    // RUNTIME
    // ==================================================

    private readonly List<InventorySlot> equippedSlots = new();
    private readonly List<InventorySlot> unequippedSlots = new();


    [SerializeField] private ItemDetailsWindow itemDetailsWindow;

    [SerializeField] private DecManager decManager;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateEquippedSlots();
    }

    private void Start()
    {
        if (_SelectionData.AllFactionDecData.Count <= 0)
        {
            decManager.InitializeAllFactionDecks();
        }

        //RefreshUI();
    }

    // ==================================================
    // SLOT CREATION
    // ==================================================

    private void CreateEquippedSlots()
    {
        equippedSlots.Clear();

        for (int i = 0; i < equippedSlotCount; i++)
        {
            InventorySlot slot = Instantiate(equippedSlotPrefab, equippedSlotsParent);

            slot.Initialize(i, SlotGroup.Equipped);
            equippedSlots.Add(slot);
        }
    }

    private InventorySlot CreateUnequippedSlot(int index)
    {
        InventorySlot slot =
            Instantiate(unequippedSlotPrefab, unequippedSlotsParent);

        slot.Initialize(index, SlotGroup.Unequipped);
        unequippedSlots.Add(slot);
        return slot;
    }

    // ==================================================
    // PUBLIC API (USED BY DecManager / Slots)
    // ==================================================

    public void AddEquippedItem(string itemId, string UnitType, Canvas _Canvas, string Details, FactionName _FName, bool Status, DecCategory category, DecSelector CurrentDec)
    {
        GameDebug.LogWarning($"Add Equipped Item ==>  Item ID: {itemId} UnitType: {UnitType}   Details: {Details} Faction: {_FName} IsEquipped: {Status}");
        if (equippedData.Count >= equippedSlotCount)
            return;

        equippedData.Add(new InventoryItemData(itemId, UnitType, _Canvas, Details, _FName, Status, category, CurrentDec));
        Debug.Log($"Added Equipped Item: {itemId} to  RefreshinhUI");
        RefreshUI();
    }

    public void AddUnequippedItem(string itemId, string UnitType, Canvas _Canvas, string Details, FactionName _FName, bool Status, DecCategory decCategory, DecSelector CurrentDec)
    {
        unequippedData.Add(new InventoryItemData(itemId, UnitType, _Canvas, Details, _FName, Status, decCategory, CurrentDec));
        RefreshUI();
    }

    public void ClearEquipped()
    {
        equippedData.Clear();
        RefreshUI();
    }

    public void ClearUnequipped()
    {
        unequippedData.Clear();
        RefreshUI();
    }

    public void RequestMove(
        SlotGroup fromGroup, int fromIndex,
        SlotGroup toGroup, int toIndex)
    {
        Debug.Log($" Request to move has been Called with From Group: {fromGroup}, from Index: {fromIndex} ToGroup: {toGroup} To Index: {toIndex}");
        if (fromGroup == toGroup)
        {
            SwapWithinGroup(fromGroup, fromIndex, toIndex);
        }
        else
        {
            Debug.Log("Moving to different group");
            MoveBetweenGroups(fromGroup, fromIndex, toGroup, toIndex);
        }
    }

    // ==================================================
    // MOVE LOGIC
    // ==================================================

    private void SwapWithinGroup(SlotGroup group, int a, int b)
    {
        Debug.Log("Swap within Group");
        var list = group == SlotGroup.Equipped
            ? equippedData
            : unequippedData;

        if (a < 0 || b < 0 || a >= list.Count || b >= list.Count)
            return;

        (list[a], list[b]) = (list[b], list[a]);
    }

    private void MoveBetweenGroups(
    SlotGroup fromGroup, int fromIndex,
    SlotGroup toGroup, int toIndex)
    {
        Debug.Log($"Move {fromGroup}[{fromIndex}] → {toGroup}[{toIndex}]");

        if (fromGroup == toGroup)
            return;

        var fromList = fromGroup == SlotGroup.Equipped
            ? equippedData
            : unequippedData;

        var toList = toGroup == SlotGroup.Equipped
            ? equippedData
            : unequippedData;

        if (fromIndex < 0 || fromIndex >= fromList.Count)
        {
            Debug.Log("Invalid fromIndex");
            return;
        }

        // Clamp destination index safely
        toIndex = Mathf.Clamp(toIndex, 0, toList.Count);

        // 🔒 Equipped full → swap instead of move
        if (toGroup == SlotGroup.Equipped &&
            toList.Count >= equippedSlotCount)
        {
            if (toIndex >= toList.Count)
                return;

            Debug.Log("Equipped full → swapping");

            var temp = toList[toIndex];
            toList[toIndex] = fromList[fromIndex];
            fromList[fromIndex] = temp;

            RefreshUI();
            return;
        }

        // ✅ Normal move
        var item = fromList[fromIndex];
        fromList.RemoveAt(fromIndex);

        // Adjust index if removing from same backing list (safety)
        if (fromList == toList && toIndex > fromIndex)
            toIndex--;

        toList.Insert(toIndex, item);

        RefreshUI();
    }



    // ==================================================
    // UI REBUILD
    // ==================================================

    private void RefreshUI()
    {
        Debug.Log("Refreshing Inventory UI...");
        ClearSlotUI(equippedSlots);
        ClearSlotUI(unequippedSlots);

        for (int i = 0; i < equippedData.Count; i++)
        {
            DraggableObject item =
                CreateUIItem(
                    equippedData[i].itemId,
                    equippedData[i].itemType,
                    equippedData[i].itemCanvas,
                    equippedData[i].description,
                    equippedData[i].factionType,
                    equippedData[i].IsEquipped,
                    equippedData[i]._Dec
                    );

            item._Item.isEquipped = true;
            equippedSlots[i].SetItem(item);
        }

        EnsureUnequippedSlots();

        for (int i = 0; i < unequippedData.Count; i++)
        {
            DraggableObject item =
                CreateUIItem(
                    unequippedData[i].itemId,
                    unequippedData[i].itemType,
                    unequippedData[i].itemCanvas,
                    unequippedData[i].description,
                    unequippedData[i].factionType,
                    unequippedData[i].IsEquipped,
                    unequippedData[i]._Dec
                    );
            item._Item.isEquipped = false;
            unequippedSlots[i].SetItem(item);
        }
    }

    private void ClearSlotUI(List<InventorySlot> slots)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
                Destroy(slot.CurrentItem.gameObject);

            slot.Clear();
        }
    }

    private void EnsureUnequippedSlots()
    {
        while (unequippedSlots.Count < unequippedData.Count)
            CreateUnequippedSlot(unequippedSlots.Count);
    }

    private DraggableObject CreateUIItem(string itemId, string UnitType, Canvas _Canvas, string Details, FactionName _FName, bool Status, DecCategory category)
    {
        GameDebug.LogWarning($"Inventory Manager ==> Create UI Item ==>  Item ID: {itemId} UnitType: {UnitType}   Details: {Details} Faction: {_FName} IsEquipped: {Status}");
        Item itemGO =
            ItemDatabase.Instance.CreateItemUI(itemId, UnitType, _Canvas, Details, _FName, itemDetailsWindow, Status, category);


        return itemGO.GetComponent<DraggableObject>();
    }

    // ==================================================
    // PLAYFAB SAVE / LOAD
    // ==================================================

    public List<InventoryItemData> GetSaveData()
    {
        List<InventoryItemData> save = new();
        save.AddRange(equippedData);
        save.AddRange(unequippedData);
        return save;
    }

    public void LoadFromData(
        List<InventoryItemData> equipped,
        List<InventoryItemData> unequipped)
    {
        equippedData = equipped;
        unequippedData = unequipped;
        RefreshUI();
    }

    public void ItemSave(DecCategory category, FactionName factionName)
    {
        foreach (var fname in _SelectionData.AllFactionDecData)
        {
            if (fname.FactionType == factionName)
            {
                if (category == DecCategory.Offense)
                {
                    fname.SelectedUnitDeck.Clear();
                    foreach (var item in equippedData)
                    {
                        fname.FactionType = item.factionType;
                        fname.SelectedUnitDeck.Add(item.Units);
                    }
                }
                else if (category == DecCategory.Defense)
                {
                    fname.SelectedDefenseDec.Clear();
                    foreach (var item in equippedData)
                    {
                        fname.FactionType = item.factionType;
                        fname.SelectedDefenseDec.Add(item.Defenses);
                    }
                }
                else if (category == DecCategory.Resource)
                {
                    fname.SelectedResourceDeck.Clear();
                    foreach (var item in equippedData)
                    {
                        fname.FactionType = item.factionType;
                        fname.SelectedResourceDeck.Add(item.Resources);
                    }
                }
            }
        }
    }
}

public enum SlotGroup
{
    Equipped,
    Unequipped
}