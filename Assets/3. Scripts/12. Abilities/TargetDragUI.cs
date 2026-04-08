using UnityEngine;
using UnityEngine.EventSystems;

public class TargetDragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool canDrag;
    public AbilityTargetingSystem targetingSystem;

    private RectTransform rect;
    private Canvas canvas;
    private Vector2 offset;
    private bool isDragging;

    public static bool IsDraggingUI;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDrag) return;

        isDragging = true;
        IsDraggingUI = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        offset = rect.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !canDrag) return;

        Vector2 screenPos = eventData.position;
        
        Ray ray = targetingSystem.mainCamera.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, targetingSystem.groundLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow, 0.1f);
            
            targetingSystem.lastRayOrigin = ray.origin;
            targetingSystem.lastRayHitPoint = hit.point;
            
            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                eventData.pressEventCamera,
                out uiPos);

            rect.anchoredPosition = uiPos + offset;
            targetingSystem.SetTargetPosition(hit.point);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        IsDraggingUI = false;
    }
}