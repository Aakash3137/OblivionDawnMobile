using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableObject : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public Item _Item;

    [HideInInspector]
    public InventorySlot CurrentSlot;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_Item.isEquipped)
        {
            eventData.pointerDrag = null;
            return;
        }

        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;

        // Move to top-most canvas automatically
        transform.SetParent(HomeUIManager.Instance.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {

        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (CurrentSlot != null)
        {

            transform.SetParent(CurrentSlot.ItemParent, false);
            rectTransform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(originalParent, false);
            rectTransform.localPosition = Vector3.zero;
        }
    }
}
