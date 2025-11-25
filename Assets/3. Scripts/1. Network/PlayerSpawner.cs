using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Inspector Fields
    
    [Header("Player Prefab")]
    public NetworkObject playerPrefab;
    
    [Header("Spawn Configuration")]
    public Vector3[] spawnPositions = { Vector3.zero, new Vector3(0, 0, 20) };
    public GameModeType gameMode = GameModeType.HostClient;
    
    #endregion
    
    #region Private Fields
    
    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    
    #endregion

    #region Network Callbacks
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] Player joined: {player}");
        
        if (runner.IsServer && !_spawnedPlayers.ContainsKey(player))
        {
            InitializeGameModeIfNeeded();
            SpawnPlayer(runner, player);
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] Player left: {player}");
        
        if (_spawnedPlayers.TryGetValue(player, out NetworkObject playerObject))
        {
            if (playerObject != null && playerObject.IsValid)
            {
                runner.Despawn(playerObject);
            }
            _spawnedPlayers.Remove(player);
        }
    }
    
    #endregion

    #region Spawning Logic
    
    private void InitializeGameModeIfNeeded()
    {
        if (_spawnedPlayers.Count == 0)
        {
            CustomGameMode.SetGameMode(gameMode);
        }
    }
    
    private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Player prefab not assigned!");
            return;
        }
        
        Vector3 spawnPosition = GetSpawnPosition();
        NetworkObject playerObject = CreatePlayerObject(runner, player, spawnPosition);
        ConfigurePlayerObject(playerObject, spawnPosition);
        RegisterPlayer(player, playerObject);
    }
    
    private Vector3 GetSpawnPosition()
    {
        int playerIndex = _spawnedPlayers.Count == 0 ? 0 : 1;
        return playerIndex < spawnPositions.Length ? spawnPositions[playerIndex] : Vector3.zero;
    }
    
    private NetworkObject CreatePlayerObject(NetworkRunner runner, PlayerRef player, Vector3 position)
    {
        return runner.Spawn(playerPrefab, position, Quaternion.identity, player);
    }
    
    private void ConfigurePlayerObject(NetworkObject playerObject, Vector3 position)
    {
        var networkPlayer = playerObject.GetComponent<NetworkPlayer>();
        if (networkPlayer != null)
        {
            networkPlayer.NetworkPosition = position;
        }
        
        DontDestroyOnLoad(playerObject.gameObject);
    }
    
    private void RegisterPlayer(PlayerRef player, NetworkObject playerObject)
    {
        _spawnedPlayers[player] = playerObject;
        Debug.Log($"[PlayerSpawner] Spawned player {player} at {playerObject.transform.position}");
    }
    
    #endregion

    #region Unused Network Callbacks
    
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
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    
    #endregion
}