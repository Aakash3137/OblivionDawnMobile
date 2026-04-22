using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect parentScroll; // Assign ScrollView A here
    private ScrollRect myScrollRect;
    private bool draggingParent;

    void Awake()
    {
        myScrollRect = GetComponent<ScrollRect>();

        if (parentScroll == null)
            parentScroll = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Compare horizontal vs vertical movement to decide who scrolls
        float xDiff = Mathf.Abs(eventData.delta.x);
        float yDiff = Mathf.Abs(eventData.delta.y);

        if (xDiff > yDiff)
        {
            draggingParent = true;
            parentScroll.OnBeginDrag(eventData);
        }
        else
        {
            draggingParent = false;
            myScrollRect.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingParent) parentScroll.OnDrag(eventData);
        else myScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingParent) parentScroll.OnEndDrag(eventData);
        else myScrollRect.OnEndDrag(eventData);
        draggingParent = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        // Forwards mouse wheel/scrollpad input
        parentScroll.OnScroll(eventData);
    }
}
