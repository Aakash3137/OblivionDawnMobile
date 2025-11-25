using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class GameSceneUI : MonoBehaviour
{
    [Header("Player 1 Info")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player1RankText;
    
    [Header("Player 2 Info")] 
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player2RankText;
    
    [Header("Camera Settings")]
    public Camera gameCamera;
    
    private bool _cameraRotationApplied = false;

    private void Start()
    {
        Debug.Log("[GameSceneUI] GameSceneUI started, waiting for players...");
        
        // ✅ FIX: Wait longer for players to be properly loaded
        Invoke(nameof(RefreshPlayerInfo), 1f);
        InvokeRepeating(nameof(RefreshPlayerInfo), 2f, 2f);
        
        // Set camera reference if not assigned
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
    }

    public void RefreshPlayerInfo()
    {
        try
        {
            // ✅ FIX: Use NetworkRunner's GetComponents for more reliable player finding
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
            
            Debug.Log($"[GameSceneUI] Found {players.Length} NetworkPlayer objects in scene");

            if (players.Length == 0)
            {
                // Try alternative method to find players
                var runner = FindObjectOfType<NetworkRunner>();
                if (runner != null)
                {
                    Debug.Log($"[GameSceneUI] NetworkRunner has {runner.ActivePlayers.ToList().Count()} active players");
                }
            }

            // Sort players consistently
            System.Array.Sort(players, (a, b) => a.Object.Id.Raw.CompareTo(b.Object.Id.Raw));

            if (players.Length > 0 && players[0] != null && players[0].IsProfileSet)
            {
                SetupPlayerUI(players[0], player1NameText, player1RankText);
                Debug.Log($"[GameSceneUI] Player 1: {players[0].GetDisplayName()}");
            }
            else
            {
                SetEmptyPlayerUI(player1NameText, player1RankText);
                Debug.Log("[GameSceneUI] Player 1 not ready");
            }

            if (players.Length > 1 && players[1] != null && players[1].IsProfileSet)
            {
                SetupPlayerUI(players[1], player2NameText, player2RankText);
                Debug.Log($"[GameSceneUI] Player 2: {players[1].GetDisplayName()}");
                
                // Check for camera rotation
                CheckCameraRotation(players[1]);
            }
            else
            {
                SetEmptyPlayerUI(player2NameText, player2RankText);
                Debug.Log("[GameSceneUI] Player 2 not ready");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameSceneUI] Error: {ex.Message}");
        }
    }

    private void SetupPlayerUI(NetworkPlayer player, TextMeshProUGUI nameText, TextMeshProUGUI rankText)
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
    }

    private void SetEmptyPlayerUI(TextMeshProUGUI nameText, TextMeshProUGUI rankText)
    {
        nameText.text = "Waiting...";
        nameText.color = Color.white;
        rankText.text = "Rank -";
    }

    private void CheckCameraRotation(NetworkPlayer player)
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
            Debug.Log($"[GameSceneUI] ✅ Camera rotated 180° for client player");
        }
        else
        {
            Debug.LogWarning($"[GameSceneUI] ❌ Game camera not assigned for rotation");
        }
    }
    
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
}