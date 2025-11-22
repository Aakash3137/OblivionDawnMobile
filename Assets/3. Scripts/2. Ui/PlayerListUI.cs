using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerListUI : MonoBehaviour
{
    public TextMeshProUGUI playerSlot1;
    public TextMeshProUGUI playerSlot2;

    public TextMeshProUGUI player1_Rank;
    public TextMeshProUGUI player2_Rank;
    private List<NetworkPlayer> _cachedPlayers = new List<NetworkPlayer>();
    private NetworkRunner _runner;

    private void Start()
    {
        _runner = PhotonNetworkManager.Instance?.Runner;
    }

    private void OnEnable()
    {
        Debug.Log("[PlayerListUI] PlayerListUI enabled");
        RefreshUI();
        
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent += HandlePlayerChange;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent += HandlePlayerChange;
            PhotonEventsHandler.Instance.OnConnectedToServerEvent += HandleConnected;
        }
        
        // More frequent refresh to catch profile synchronization
        InvokeRepeating(nameof(RefreshUI), 0.3f, 0.5f);
    }

    private void OnDisable()
    {
        Debug.Log("[PlayerListUI] PlayerListUI disabled");
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent -= HandlePlayerChange;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent -= HandlePlayerChange;
            PhotonEventsHandler.Instance.OnConnectedToServerEvent -= HandleConnected;
        }
        CancelInvoke(nameof(RefreshUI));
    }

    private void HandleConnected()
    {
        Debug.Log("[PlayerListUI] Connected to server, refreshing UI");
        Invoke(nameof(RefreshUI), 0.5f);
    }

    private void HandlePlayerChange(PlayerRef player)
    {
        Debug.Log($"[PlayerListUI] Player change detected: {player}, refreshing UI");
        // Multiple refreshes to ensure we catch the RPC synchronization
        Invoke(nameof(RefreshUI), 0.1f);
        Invoke(nameof(RefreshUI), 0.3f);
        Invoke(nameof(RefreshUI), 0.7f);
    }

    internal void RefreshUI()
    {
        try
        {
            NetworkPlayer[] allPlayers = FindObjectsOfType<NetworkPlayer>();
            List<NetworkPlayer> validPlayers = new List<NetworkPlayer>();

            foreach (var player in allPlayers)
            {
                if (player != null)
                {
                    validPlayers.Add(player);
                }
            }

            // Sort players by their ID for consistent ordering
            validPlayers.Sort((a, b) => a.Object.Id.Raw.CompareTo(b.Object.Id.Raw));

            int totalPlayers = validPlayers.Count;
            int playersWithProfile = 0;

            // Count players with profiles
            foreach (var player in validPlayers)
            {
                if (player.IsProfileSet)
                {
                    playersWithProfile++;
                }
            }

            Debug.Log($"[PlayerListUI] Found {totalPlayers} players, {playersWithProfile} with profiles");

            // Update Player 1 slot
            if (totalPlayers >= 1)
            {
                var player1 = validPlayers[0];
                if (player1.IsProfileSet)
                {
                    playerSlot1.text = player1.GetDisplayInfo();
                    player1_Rank.text = player1.GetRank().ToString();
                }
                else
                {
                    playerSlot1.text = "Connecting...";
                    player1_Rank.text = "";
                }
            }
            else
            {
                playerSlot1.text = "";
                player1_Rank.text = "";
            }

            // Update Player 2 slot - CRITICAL FIX for the join scenario
            if (totalPlayers >= 2)
            {
                var player2 = validPlayers[1];
                if (player2.IsProfileSet)
                {
                    playerSlot2.text = player2.GetDisplayInfo();
                    player2_Rank.text = player2.GetRank().ToString();
                }
                else
                {
                    // Player 2 is connected but profile not set yet
                    playerSlot2.text = "Connecting...";
                    player2_Rank.text = "";
                }
            }
            else if (totalPlayers == 1)
            {
                // Only one player - check if we should show Player 1's info in both slots
                var player1 = validPlayers[0];
                if (player1.IsProfileSet)
                {
                    // For the joining player: show Player 1's info and "Connecting..." for themselves
                    if (_runner != null && _runner.IsClient)
                    {
                        playerSlot2.text = "Connecting...";
                    }
                    else
                    {
                        playerSlot2.text = "Waiting for player...";
                    }
                }
                else
                {
                    playerSlot2.text = "Waiting for player...";
                }
                player2_Rank.text = "";
            }
            else
            {
                playerSlot2.text = "";
                player2_Rank.text = "";
            }

            // Detailed debug info
            for (int i = 0; i < validPlayers.Count; i++)
            {
                var player = validPlayers[i];
                string profileStatus = player.IsProfileSet ? $"Name: {player.PlayerName}, Rank: {player.Rank}" : "Profile not set";
                Debug.Log($"[PlayerListUI] Player {i+1}: Ref={player.Object.InputAuthority}, {profileStatus}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[PlayerListUI] Error in RefreshUI: {ex.Message}");
        }
    }

    // Public method to force refresh from other scripts
    public void ForceRefresh()
    {
        RefreshUI();
    }
}