using UnityEngine;
using TMPro;

public class TileCounterUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerText;
    public TMP_Text enemyText;

    private void Start()
    {
        CubeGridManager.Instance.onTileOccupied += UpdateUI;
    }

    private void UpdateUI(int playerCount, int enemyCount)
    {
        if (playerText != null)
            playerText.SetText($"{playerCount}");

        if (enemyText != null)
            enemyText.SetText($"{enemyCount}");
    }

    private void OnDestroy()
    {
        if (CubeGridManager.Instance != null)
            CubeGridManager.Instance.onTileOccupied -= UpdateUI;
    }
}
