using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

/// <summary>
/// Full INetworkRunnerCallbacks implementation that re-exposes events for game systems.
/// Keep methods implemented even if empty to satisfy the interface.
/// </summary>
public class PhotonEventsHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PhotonEventsHandler Instance { get; private set; }

    public event Action<PlayerRef> OnPlayerJoinedEvent;
    public event Action<PlayerRef> OnPlayerLeftEvent;
    public event Action OnConnectedToServerEvent;
    public event Action<NetDisconnectReason> OnDisconnectedFromServerEvent;

    private void Awake()
    {
        // ✅ Proper singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ Keep alive between scenes
        Debug.Log("[PEH] PhotonEventsHandler initialized");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[PEH] Connected to server");
        OnConnectedToServerEvent?.Invoke();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning($"[PEH] Disconnected: {reason}");
        OnDisconnectedFromServerEvent?.Invoke(reason);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Accept all incoming connections for private lobbies.
        request.Accept();
        Debug.Log("[PEH] ConnectRequest accepted");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"[PEH] Connect failed: {reason}");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("[PEH] CustomAuthResponse");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PEH] OnPlayerJoined called - Runner: {runner != null}, Player: {player}");
        OnPlayerJoinedEvent?.Invoke(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PEH] PlayerLeft: {player}");
        OnPlayerLeftEvent?.Invoke(player);
        
        // Handle PvP disconnections
        if (CustomGameMode.CurrentGameMode == GameModeType.PvP)
        {
            Debug.Log("[PEH] PvP player disconnect detected");
            PhotonNetworkManager.Instance?.HandleOpponentDisconnect();
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.LogWarning($"[PEH] Shutdown: {reason}");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("[PEH] Host migration");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // optional: handle large reliable transfers
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("[PEH] Scene load start"); }
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("[PEH] Scene load done"); }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
}
