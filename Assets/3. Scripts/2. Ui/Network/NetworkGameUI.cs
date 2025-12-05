using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkGameUI : MonoBehaviour
{
    [Header("Player 1 Info")]
    [SerializeField] internal TextMeshProUGUI player1NameText;
    [SerializeField] internal TextMeshProUGUI player1RankText;
    [SerializeField] internal TextMeshProUGUI player1TileCount;
    
    [Header("Player 2 Info")] 
    [SerializeField] internal TextMeshProUGUI player2NameText;
    [SerializeField] internal TextMeshProUGUI player2RankText;
    [SerializeField] internal TextMeshProUGUI player2TileCount;
    
    internal Color playerColor = Color.green;
    internal Color enemyColor = Color.red;
    
    private bool _cameraRotationApplied = false;

    private void Start()
    {
        Debug.Log("[PrivateLobbyUI] PrivateLobbyUI started, waiting for players...");
        
        // ✅ FIX: Wait longer for players to be properly loaded
        Invoke(nameof(RefreshPlayerInfo), 1f);
        InvokeRepeating(nameof(RefreshPlayerInfo), 1f, 2f);
        
        // Set camera reference if not assigned
       /* if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }*/
    }

    public void RefreshPlayerInfo()
    {
        try
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
           // Debug.Log($"[NetworkGameUI] Found {players.Length} NetworkPlayer objects in scene");

            NetworkPlayer localPlayer = players.FirstOrDefault(p => p.Object.HasInputAuthority);
            NetworkPlayer enemyPlayer = players.FirstOrDefault(p => !p.Object.HasInputAuthority);

            // Local player always on left (Player 1 UI)
            if (localPlayer != null && localPlayer.IsProfileSet)
            {
                SetupPlayerUI(localPlayer, player1NameText, player1RankText, player1TileCount, playerColor, true);
            }
            else
            {
                SetEmptyPlayerUI(player1NameText, player1RankText);
            }
            
            // Enemy player always on Right (Player 2 UI)
            if (enemyPlayer != null && enemyPlayer.IsProfileSet)
            {
                SetupPlayerUI(enemyPlayer, player2NameText, player2RankText, player2TileCount, enemyColor, false);
            }
            else
            {
                SetEmptyPlayerUI(player2NameText, player2RankText);
            }
        }
        catch (System.Exception ex)
        {
           // Debug.LogWarning($"[NetworkGameUI] Error: {ex.Message}");
        }
    }


    private void SetupPlayerUI(NetworkPlayer player, TextMeshProUGUI nameText, TextMeshProUGUI rankText,TextMeshProUGUI tileCount, Color color, bool isPlayer)
    {
        if (player != null && player.IsProfileSet)
        {
            nameText.text = player.GetDisplayName();
            nameText.color = color;
            rankText.text = $"Rank: {player.GetRank()}";
            if (isPlayer)
            {
                tileCount.text = $"Tiles: {NetworkHexGridManager.Instance.playerTileCount}";
                tileCount.color = color;
            }
            else
            {
                tileCount.text = $"Tiles: {NetworkHexGridManager.Instance.playerTileCount}";
                tileCount.color = color;
            }
        }
        else
        {
            SetEmptyPlayerUI(nameText, rankText);
        }
    }

    //player leave ui update
    internal void SetEmptyPlayerUI(TextMeshProUGUI nameText, TextMeshProUGUI rankText, TextMeshProUGUI tileCount = null)
    {
        nameText.text = "Waiting...";
        nameText.color = Color.white;
        rankText.text = "Rank -";
        tileCount.text = "Tiles -";
    }

   
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
}