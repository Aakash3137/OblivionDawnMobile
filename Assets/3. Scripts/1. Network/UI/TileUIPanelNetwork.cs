using UnityEngine;
using UnityEngine.UI;

public class TileUIPanelNetwork : MonoBehaviour
{
    [Header("UI")]
    public Button BackButton;

    private NetworkTile currentTile;
    private FloatingUiManagerNetwork manager;

    private void Awake()
    {
        manager = GetComponentInParent<FloatingUiManagerNetwork>();
    }

    private void Start()
    {
        if (BackButton != null)
            BackButton.onClick.AddListener(OnBackClicked);
    }

    public void Open(NetworkTile tile)
    {
        currentTile = tile;
    }

    private void OnBackClicked()
    {
        currentTile = null;

        if (manager != null)
            manager.CloseUI();
    }
}