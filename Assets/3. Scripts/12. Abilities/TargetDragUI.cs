using UnityEngine;
using UnityEngine.EventSystems;

public class TargetDragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool canDrag;

    private RectTransform rect;
    private Canvas canvas;
    private Vector2 offset;
    private bool isDragging;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDrag) return;

        isDragging = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        offset = rect.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !canDrag) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 globalPoint))
        {
            rect.anchoredPosition = globalPoint + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}