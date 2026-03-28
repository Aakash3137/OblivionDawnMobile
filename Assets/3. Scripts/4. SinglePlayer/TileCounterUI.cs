using UnityEngine;
using TMPro;

public class TileCounterUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerText;
    public TMP_Text enemyText;


    private void OnEnable()
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

    private void OnDisable()
    {
        if (CubeGridManager.Instance != null)
            CubeGridManager.Instance.onTileOccupied -= UpdateUI;
    }
}
