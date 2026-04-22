using Sirenix.OdinInspector;
using UnityEngine;

public class UIPanelSizeHandler : MonoBehaviour
{
    [SerializeField] private bool resizePanel;
    [ShowIf("resizePanel")]
    [SerializeField] private Vector2 maxSize;
    private RectTransform rectTransform;
    RectTransform canvasRect;

    private void Awake()
    {
        TryGetComponent(out rectTransform);

        var canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvasRect = canvas.rootCanvas.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (resizePanel)
            SetRectSize();

        var centerScrollHandler = GetComponentInChildren<CenterScrollHandler>();

        if (centerScrollHandler != null)
            centerScrollHandler.SetScrollContentSize();
    }

    public void SetRectSize()
    {
        var width = Mathf.Min(maxSize.x, canvasRect.rect.width);
        var height = Mathf.Min(maxSize.y, canvasRect.rect.height);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
