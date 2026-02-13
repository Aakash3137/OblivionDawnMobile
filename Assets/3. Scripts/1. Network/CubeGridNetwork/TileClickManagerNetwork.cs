using UnityEngine;

public class TileClickManagerNetwork : MonoBehaviour
{
    [Header("References")] public GameObject BuildUIPanel; // assign prefab with UI CanvasGroup + TileUIPanel
    private CanvasGroup currentGroup;
    private TileUIPanelNetwork currentPanel;
    
    private Camera mainCamera;
    private NetworkTile lastShownTile;
    
    private bool isClosing;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("MainCamera not found! Make sure your camera has the 'MainCamera' tag.");
            
        CloseUI(); // hide at start
    }

    void Update()
    {
        if (TileSelectionManager.Instance != null && TileSelectionManager.Instance.selectedTile != null)
        {
            var tile = TileSelectionManager.Instance.selectedTile;
            if (!tile.IsOccupied && tile.CurrentVisualOwner == NetworkSide.Player)
            {
                if (lastShownTile != tile)
                {
                    ShowUI(tile);
                    lastShownTile = tile;
                }
            }
        }
        else if (lastShownTile != null && !isClosing)
        {
            CloseUI();
            lastShownTile = null;
        }
    }
    public void ShowUI(NetworkTile tile)
    {
        // If we haven't created the UI yet, do it once
        BuildUIPanel.SetActive(true);
        currentPanel = BuildUIPanel.GetComponent<TileUIPanelNetwork>();
        currentGroup = BuildUIPanel.GetComponent<CanvasGroup>();

        // Update panel content
        if (currentPanel != null)
            currentPanel.Open(tile);
        
    }

    public void CloseUI()
    {
        if (BuildUIPanel == null || isClosing)
            return;

            isClosing = true;
            BuildUIPanel.SetActive(false);
    }
    
}
