using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkGameUI : MonoBehaviour
{
    [Header("Player 1 Info")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player1RankText;
    
    [Header("Player 2 Info")] 
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player2RankText;
    
   // [Header("Camera Settings")]
    //public Camera gameCamera;
    
    private bool _cameraRotationApplied = false;

    private void Start()
    {
        Debug.Log("[PrivateLobbyUI] PrivateLobbyUI started, waiting for players...");
        
        // ✅ FIX: Wait longer for players to be properly loaded
        Invoke(nameof(RefreshPlayerInfo), 1f);
        InvokeRepeating(nameof(RefreshPlayerInfo), 2f, 2f);
        
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
            Debug.Log($"[NetworkGameUI] Found {players.Length} NetworkPlayer objects in scene");

            NetworkPlayer localPlayer = players.FirstOrDefault(p => p.Object.HasInputAuthority);
            NetworkPlayer enemyPlayer = players.FirstOrDefault(p => !p.Object.HasInputAuthority);

            // Local player always on left (Player 1 UI)
            if (localPlayer != null && localPlayer.IsProfileSet)
            {
                SetupPlayerUI(localPlayer, player1NameText, player1RankText, Color.green);
                Debug.Log($"[NetworkGameUI] Player 1 (Local): {localPlayer.GetDisplayName()}");
            }
            else
            {
                SetEmptyPlayerUI(player1NameText, player1RankText);
                Debug.Log("[NetworkGameUI] Player 1 (Local) not ready");
            }

            // Enemy player always on right (Player 2 UI)
            if (enemyPlayer != null && enemyPlayer.IsProfileSet)
            {
                SetupPlayerUI(enemyPlayer, player2NameText, player2RankText, Color.red);
                Debug.Log($"[NetworkGameUI] Player 2 (Enemy): {enemyPlayer.GetDisplayName()}");
            }
            else
            {
                SetEmptyPlayerUI(player2NameText, player2RankText);
                Debug.Log("[NetworkGameUI] Player 2 (Enemy) not ready");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[NetworkGameUI] Error: {ex.Message}");
        }
    }


    /* private void SetupPlayerUI(NetworkPlayer player, TextMeshProUGUI nameText, TextMeshProUGUI rankText)
     {
         if (player != null && player.IsProfileSet)
         {
             nameText.text = player.GetDisplayName();
             nameText.color = player.GetPlayerColor();
             rankText.text = $"Rank {player.GetRank()}";
         }
         else
         {
             SetEmptyPlayerUI(nameText, rankText);
         }
     }*/
    private void SetupPlayerUI(NetworkPlayer player, TextMeshProUGUI nameText, TextMeshProUGUI rankText, Color color)
    {
        if (player != null && player.IsProfileSet)
        {
            nameText.text = player.GetDisplayName();
            nameText.color = color;
            rankText.text = $"Rank: {player.GetRank()}";
        }
        else
        {
            SetEmptyPlayerUI(nameText, rankText);
        }
    }


    internal void SetEmptyPlayerUI(TextMeshProUGUI nameText, TextMeshProUGUI rankText)
    {
        nameText.text = "Waiting...";
        nameText.color = Color.white;
        rankText.text = "Rank -";
    }

    /*private void CheckCameraRotation(NetworkPlayer player)
    {
        // Rotate camera once for second player (client) in both Host/Client and PvP modes
        if (!_cameraRotationApplied && player.Object.HasInputAuthority && player.PlayerColorIndex == 1)
        {
            RotateClientCamera();
            _cameraRotationApplied = true;
        }
    }
    
    private void RotateClientCamera()
    {
        if (gameCamera != null)
        {
            Vector3 cameraRotation = gameCamera.transform.eulerAngles;
            cameraRotation.z = 180f;
            gameCamera.transform.eulerAngles = cameraRotation;
            Debug.Log($"[PrivateLobbyUI] ✅ Camera rotated 180° for client player");
        }
        else
        {
            Debug.LogWarning($"[PrivateLobbyUI] ❌ Game camera not assigned for rotation");
        }
    }*/
    
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
}