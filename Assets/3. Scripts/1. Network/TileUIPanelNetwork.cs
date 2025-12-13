using System;
using UnityEngine;
using UnityEngine.UI;

public class TileUIPanelNetwork : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;

    public Button BackButton;
    
    private NetworkTile currentTile;

    public void Start()
    {
        BackButton.onClick.AddListener(() => Close());
    }

    public void Open(NetworkTile tile)
    {
        currentTile = tile;
        ShowPanel(true);
    }

    public void Close()
    {
        currentTile = null;
        ShowPanel(false);
    }



    private void ShowPanel(bool show)
    {
        // FloatingUiManagerNetwork handles alpha/fade
        // Just manage interactable state here
        if (canvasGroup != null)
        {
            canvasGroup.interactable = show;
        }
    }

}
