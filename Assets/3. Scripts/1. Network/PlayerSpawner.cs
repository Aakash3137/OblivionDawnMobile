using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Assign your NetworkPlayer prefab here")]
    public NetworkObject playerPrefab;

    [Header("Spawn Positions")]
    public Vector3[] spawnPositions = { Vector3.zero, new Vector3(0, 0, 20) };

    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[SimpleSpawner] Player joined: {player}");
        
        if (runner.IsServer && !_spawnedPlayers.ContainsKey(player))
        {
            SpawnPlayer(runner, player);
        }
    }

    private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[SimpleSpawner] ❌ Player prefab is not assigned! Please assign it in the inspector.");
            return;
        }

        // Always spawn second player at position 1 (0,0,20)
        int playerIndex = _spawnedPlayers.Count == 0 ? 0 : 1;
        Vector3 spawnPosition = playerIndex < spawnPositions.Length ? spawnPositions[playerIndex] : Vector3.zero;
        
        Debug.Log($"[SimpleSpawner] Player {player} (ID: {player.PlayerId}) -> Index: {playerIndex} -> Position: {spawnPosition}");
        
        NetworkObject playerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        
        Debug.Log($"[SimpleSpawner] Spawned at position: {playerObject.transform.position}");
        
        DontDestroyOnLoad(playerObject.gameObject);
        
        _spawnedPlayers[player] = playerObject;
        Debug.Log($"[SimpleSpawner] ✅ Spawned Persistent NetworkPlayer for {player} with ID: {playerObject.Id}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[SimpleSpawner] Player left: {player}");
        
        if (_spawnedPlayers.TryGetValue(player, out NetworkObject playerObject))
        {
            if (playerObject != null && playerObject.IsValid)
            {
                runner.Despawn(playerObject);
            }
            _spawnedPlayers.Remove(player);
        }
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    
    // Fixed: Added the missing reliable data methods
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}