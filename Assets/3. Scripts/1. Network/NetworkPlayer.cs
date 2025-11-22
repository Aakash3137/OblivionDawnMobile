using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public int Rank { get; set; }
    [Networked] public bool IsProfileSet { get; set; }
    [Networked] public int PlayerColorIndex { get; set; }

    private PlayerProfile _playerProfile;
    private MeshRenderer _meshRenderer;

    public override void Spawned()
    {
        base.Spawned();

        Debug.Log($"[NetworkPlayer] Spawned - InputAuthority: {Object.InputAuthority}, Scene: {gameObject.scene.name}");
        DontDestroyOnLoad(gameObject);

        if (Object.HasInputAuthority && !IsProfileSet)
        {
            SetPlayerProfile();
        }
        else if (IsProfileSet)
        {
            RefreshPlayerUI();
        }

        Invoke(nameof(DelayedRefresh), 0.5f);
        
        // Set player color
        if (Object.HasStateAuthority)
        {
            SetNetworkPlayerColor();
        }
        SetPlayerColor();
        
        // ✅ FIX: Subscribe to scene changes
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[NetworkPlayer] Scene changed to: {scene.name}, refreshing UI");
        Invoke(nameof(DelayedRefresh), 0.3f);
    }

    private void DelayedRefresh()
    {
        RefreshPlayerUI();
    }

    private void SetPlayerProfile()
    {
        _playerProfile = CreatePlayerProfile();
        RPC_SetPlayerProfile(_playerProfile.PlayerName, _playerProfile.Rank);
    }

    private PlayerProfile CreatePlayerProfile()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        var profile = new PlayerProfile();
        profile.InitializeWithRandomData();
        return profile;
        #else
        return PlayerProfile.LoadFromDisk();
        #endif
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetPlayerProfile(string name, int rank)
    {
        PlayerName = name;
        Rank = rank;
        IsProfileSet = true;
        
        Debug.Log($"[NetworkPlayer] 🎯 Profile set: {name} (Rank: {rank}) for {Object.InputAuthority}");
        
        RefreshPlayerUI();
    }

    public PlayerProfile GetPlayerProfile()
    {
        if (_playerProfile == null && IsProfileSet)
        {
            _playerProfile = new PlayerProfile
            {
                PlayerName = PlayerName.ToString(),
                Rank = Rank
            };
        }
        return _playerProfile;
    }

    internal void RefreshPlayerUI()
    {
        RefreshLobbyUI();
        RefreshGameUI();
    }

    private void RefreshLobbyUI()
    {
        // Only refresh lobby UI if we're in a lobby scene
        if (gameObject.scene.name == "MainScene" || gameObject.scene.name == "LobbyScene")
        {
            var playerListUI = FindObjectOfType<PlayerListUI>();
            if (playerListUI != null)
            {
                playerListUI.RefreshUI();
            }
        }
    }

    private void RefreshGameUI()
    {
        // Only refresh game UI if we're in a game scene
        if (gameObject.scene.name == "GameScene")
        {
            var gameUI = FindObjectOfType<GameSceneUI>();
            if (gameUI != null)
            {
                gameUI.RefreshPlayerInfo();
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // ✅ FIX: Clean up scene change subscription
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        base.Despawned(runner, hasState);
    }

    public string GetDisplayName()
    {
        return IsProfileSet ? PlayerName.ToString() : "Connecting...";
    }
    
    public int GetRank()
    {
        return Rank;
    }
    
    public string GetDisplayInfo()
    {
        if (!IsProfileSet || string.IsNullOrEmpty(PlayerName.ToString()))
        {
            return "Connecting...";
        }
        return $"{PlayerName.ToString()} [{Rank}]";
    }

    private void SetNetworkPlayerColor()
    {
        PlayerColorIndex = Object.InputAuthority.PlayerId - 1;
        Debug.Log($"[NetworkPlayer] Player ID: {Object.InputAuthority.PlayerId}, ColorIndex: {PlayerColorIndex}");
    }

    /*private void SetPlayerColor()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            Color playerColor = Object.HasInputAuthority ? Color.green : Color.red;
            _meshRenderer.material.color = playerColor;
        }
    }*/
    private void SetPlayerColor()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            Color playerColor = PlayerColorIndex == 0 ? Color.green : Color.red;
            _meshRenderer.material.color = playerColor;
            Debug.Log($"[NetworkPlayer] Setting color for Player ID {Object.InputAuthority.PlayerId}: {playerColor}");
        }
    }

    public Color GetPlayerColor()
    {
        return PlayerColorIndex == 0 ? Color.green : Color.red;
    }
}