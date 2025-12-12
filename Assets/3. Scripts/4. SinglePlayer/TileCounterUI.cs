// using UnityEngine;
// using TMPro;

// public class TileCounterUI : MonoBehaviour
// {
//     [Header("UI Reference")]
//     public TMP_Text sideText;   // Assign the TMP_Text in Inspector

//     [Header("Side Settings")]
//     public Side targetSide;     // Choose Player or Enemy in Inspector

//     [Header("Update Settings")]
//     public float updateInterval = 0.5f; // How often to refresh count

//     private float timer;

//     void Update()
//     {
//         timer += Time.deltaTime;
//         if (timer >= updateInterval)
//         {
//             UpdateTileCount();
//             timer = 0f;
//         }
//     }

//     void UpdateTileCount()
//     {
//         if (CubeGridManager.Instance == null || sideText == null) return;

//         int count = 0;
//         foreach (var kvp in CubeGridManager.Instance.cubeTiles)
//         {
//             Tile tile = kvp.Value.GetComponent<Tile>();
//             if (tile != null && tile.ownerSide == targetSide)
//             {
//                 count++;
//             }
//         }

//         sideText.text = $"{targetSide} Tiles: {count}";
//     }
// }





using UnityEngine;
using TMPro;

public class TileCounterUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerText;
    public TMP_Text enemyText;

    private int playerCount = 0;
    private int enemyCount = 0;

    public static TileCounterUI Instance; // Singleton for easy access

    void Awake()
    {
        Instance = this;
    }

    public void OnTileOwnerChanged(Side oldOwner, Side newOwner)
    {
        // Decrease old owner count
        if (oldOwner == Side.Player) playerCount--;
        else if (oldOwner == Side.Enemy) enemyCount--;

        // Increase new owner count
        if (newOwner == Side.Player) playerCount++;
        else if (newOwner == Side.Enemy) enemyCount++;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerText != null)
            playerText.text = $"Player Tiles: {playerCount}";

        if (enemyText != null)
            enemyText.text = $"Enemy Tiles: {enemyCount}";
    }

    // Optional: initialize counts at start
    public void InitializeCounts()
    {
        playerCount = 0;
        enemyCount = 0;

        foreach (var kvp in CubeGridManager.Instance.cubeTiles)
        {
            Tile tile = kvp.Value.GetComponent<Tile>();
            if (tile == null) continue;

            if (tile.ownerSide == Side.Player) playerCount++;
            else if (tile.ownerSide == Side.Enemy) enemyCount++;
        }

        UpdateUI();
    }
}
