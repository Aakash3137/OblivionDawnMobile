using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Info (Set by Manager)")]
    public int SlotIndex { get; private set; }
    public SlotGroup Group { get; private set; }

    [Header("UI")]
    [SerializeField] private Transform itemContainer;
    public Transform ItemParent => transform.GetChild(0);

    public DraggableObject CurrentItem { get; private set; }

    // ----------------------------------------------------
    // INITIALIZATION (Called by InventoryManager)
    // ----------------------------------------------------
    public void Initialize(int index, SlotGroup group)
    {
        SlotIndex = index;
        Group = group;

        if (itemContainer == null && transform.childCount > 0)
            itemContainer = transform.GetChild(0);
    }

    // ----------------------------------------------------
    // DROP HANDLING
    // ----------------------------------------------------
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DraggableObject dragged =
            eventData.pointerDrag.GetComponent<DraggableObject>();

        Debug.Log($"Dragged Object: {dragged} && Dragged Item: {dragged._Item.name} Status: {dragged._Item.isEquipped}");

        if (dragged == null)
            return;

        if (dragged._Item != null && dragged._Item.isEquipped)
            return;

        if (dragged.CurrentSlot == this)
            return;

        dragged._Item.isEquipped = true;
        CurrentItem._Item.isEquipped = false;

        InventoryManager.Instance.RequestMove(
            dragged.CurrentSlot.Group,
            dragged.CurrentSlot.SlotIndex,
            Group,
            SlotIndex
        );
    }

    // ----------------------------------------------------
    // UI MANAGEMENT (CALLED BY MANAGER ONLY)
    // ----------------------------------------------------
    public void SetItem(DraggableObject item)
    {
//        Debug.Log("Set Item "+ item.name);
        CurrentItem = item;

        if (item == null)
            return;

        item.transform.SetParent(itemContainer, false);
        item.transform.localPosition = Vector3.zero;
        item.CurrentSlot = this;
    }

    public void Clear()
    {
        CurrentItem = null;
    }

    public bool IsEmpty => CurrentItem == null;
}
