using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastBlocker : MonoBehaviour, IPointerDownHandler
{
    public static bool ClickConsumedThisFrame;

    public void OnPointerDown(PointerEventData eventData)
    {
        ClickConsumedThisFrame = true;
    }
}