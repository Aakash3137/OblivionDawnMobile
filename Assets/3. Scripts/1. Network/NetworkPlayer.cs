using Fusion;
using UnityEngine;

/// <summary>
/// Networked player representation that handles player data synchronization,
/// visual appearance, and UI updates across multiplayer sessions.
/// </summary>
public class NetworkPlayer : NetworkBehaviour
{
    #region Networked Properties
    
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public int Rank { get; set; }
    [Networked] public bool IsProfileSet { get; set; }
    [Networked] public int PlayerColorIndex { get; set; }
    [Networked] public Vector3 NetworkPosition { get; set; }
    
    #endregion
    
    #region Private Fields
    
    private PlayerProfile _playerProfile;
    private bool _isMatchmakingMode;
    private MeshRenderer _meshRenderer;
    
    #endregion

    #region Unity Lifecycle & Network Events
    
    public override void Spawned()
    {
        base.Spawned();
        
        Debug.Log($"[NetworkPlayer] Spawned - InputAuthority: {Object.InputAuthority}, Scene: {gameObject.scene.name}");
        DontDestroyOnLoad(gameObject);
        
        InitializeNetworkPosition();
        InitializePlayerProfile();
        InitializeVisualAppearance();
        InitializeGameMode();
        SubscribeToEvents();
        
        // Delayed UI refresh to ensure all components are ready
        Invoke(nameof(DelayedRefresh), 0.5f);
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        UnsubscribeFromEvents();
        base.Despawned(runner, hasState);
    }
    
    public override void FixedUpdateNetwork()
    {
        // Synchronize position for non-authoritative clients
        if (!Object.HasStateAuthority)
        {
            transform.position = NetworkPosition;
        }
    }
    
    #endregion

    #region Initialization Methods
    
    private void InitializeNetworkPosition()
    {
        if (Object.HasStateAuthority)
        {
            NetworkPosition = transform.position;
        }
    }
    
    private void InitializePlayerProfile()
    {
        if (Object.HasInputAuthority && !IsProfileSet)
        {
            SetPlayerProfile();
        }
        else if (IsProfileSet)
        {
            RefreshPlayerUI();
        }
    }
    
    private void InitializeVisualAppearance()
    {
        if (Object.HasStateAuthority)
        {
            SetNetworkPlayerColor();
        }
        SetPlayerColor();
    }
    
    private void InitializeGameMode()
    {
        _isMatchmakingMode = CustomGameMode.IsMatchmakingGame;
    }
    
    private void SubscribeToEvents()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void UnsubscribeFromEvents()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    #endregion

    #region Event Handlers
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[NetworkPlayer] Scene changed to: {scene.name}, refreshing UI");
        Invoke(nameof(DelayedRefresh), 0.3f);
    }
    
    private void DelayedRefresh()
    {
        RefreshPlayerUI();
    }
    
    #endregion
    
    #region Player Profile Management
    
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
        
        Debug.Log($"[NetworkPlayer] Profile set: {name} (Rank: {rank}) for {Object.InputAuthority}");
        RefreshPlayerUI();
    }
    
    #endregion

    #region UI Management
    
    internal void RefreshPlayerUI()
    {
        RefreshLobbyUI();
        RefreshGameUI();
    }
    
    private void RefreshLobbyUI()
    {
        // Update lobby UI for main/lobby scenes
        if (gameObject.scene.name == "MainScene" || gameObject.scene.name == "LobbyScene")
        {
            var playerListUI = FindObjectOfType<PlayerListUI>();
            playerListUI?.RefreshUI();
        }
    }
    
    private void RefreshGameUI()
    {
        // Update game scene UI
        if (gameObject.scene.name == "GameScene")
        {
            var gameUI = FindObjectOfType<GameSceneUI>();
            gameUI?.RefreshPlayerInfo();
        }
        
        // Update PvP matchmaking UI
        if (_isMatchmakingMode)
        {
            var pvpUI = FindObjectOfType<PvPMatchUI>();
            pvpUI?.RefreshPlayerInfo();
        }
    }
    
    #endregion

    #region Visual Appearance
    
    private void SetNetworkPlayerColor()
    {
        PlayerColorIndex = Object.InputAuthority.PlayerId - 1;
        Debug.Log($"[NetworkPlayer] Player ID: {Object.InputAuthority.PlayerId}, ColorIndex: {PlayerColorIndex}");
    }
    
    private void SetPlayerColor()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            // Each player sees themselves as green, enemies as red
            Color playerColor = Object.HasInputAuthority ? Color.green : Color.red;
            _meshRenderer.material.color = playerColor;
            Debug.Log($"[NetworkPlayer] Color set - IsLocal: {Object.HasInputAuthority}, Color: {playerColor}");
        }
    }
    
    #endregion

    #region Public API
    
    /// <summary>
    /// Gets the player profile, creating it from networked data if needed.
    /// </summary>
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
    
    /// <summary>
    /// Gets the display name for UI purposes.
    /// </summary>
    public string GetDisplayName()
    {
        return IsProfileSet ? PlayerName.ToString() : "Connecting...";
    }
    
    /// <summary>
    /// Gets the player's rank.
    /// </summary>
    public int GetRank()
    {
        return Rank;
    }
    
    /// <summary>
    /// Gets formatted display information including name and rank.
    /// </summary>
    public string GetDisplayInfo()
    {
        if (!IsProfileSet || string.IsNullOrEmpty(PlayerName.ToString()))
        {
            return "Connecting...";
        }
        return $"{PlayerName.ToString()} [{Rank}]";
    }
    
    /// <summary>
    /// Gets the player's color from their perspective (always green for self, red for others).
    /// </summary>
    public Color GetPlayerColor()
    {
        return Object.HasInputAuthority ? Color.green : Color.red;
    }
    
    #endregion
}