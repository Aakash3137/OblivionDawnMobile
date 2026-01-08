using UnityEngine;

public class BuildPanelManager : MonoBehaviour
{
    private TileUIPanel tileUIPanel;
    [SerializeField] private RectTransform canvasRect;
    private RectTransform buildPanelTransform;
    private RectTransform buildPanelContainer;
    [SerializeField] private Vector2 yOffset = new Vector3(0, 250f);

    private void Awake()
    {
        tileUIPanel = GetComponent<TileUIPanel>();
        buildPanelTransform = GetComponent<RectTransform>();
        buildPanelContainer = transform.parent.GetComponent<RectTransform>();
        CloseBuildPanel();
    }
    public void OpenBuildPanel(Tile tile)
    {
        if (tileUIPanel != null)
            tileUIPanel.Open(tile);

        Vector3 mousePosition = Input.mousePosition;
        SetPanelPosition(mousePosition);
        Clamp();
    }

    public void CloseBuildPanel()
    {
        tileUIPanel.Close();
    }

    private void SetPanelPosition(Vector2 mousePosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePosition,
            null,
            out localPoint
        );

        buildPanelTransform.anchoredPosition = localPoint + yOffset;
    }

    private void Clamp()
    {
        Vector2 panelPosition = buildPanelTransform.anchoredPosition;

        Vector2 min = buildPanelContainer.rect.min - buildPanelTransform.rect.min;
        Vector2 max = buildPanelContainer.rect.max - buildPanelTransform.rect.max;

        panelPosition.x = Mathf.Clamp(panelPosition.x, min.x, max.x);
        panelPosition.y = Mathf.Clamp(panelPosition.y, min.y, max.y);

        buildPanelTransform.anchoredPosition = panelPosition;

    }

}
