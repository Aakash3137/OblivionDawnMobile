using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonNetworkManager : MonoBehaviour
{
    public static PhotonNetworkManager Instance { get; private set; }

    [Header("References")]
    public PhotonEventsHandler eventsHandlerPrefab;
    
    public bool AutoStart = false;
    public string LastSessionName;

    private NetworkRunner _runner;
    public NetworkRunner Runner => _runner;

    public event Action<string> OnLobbyCreated;
    public event Action OnLobbyJoined;

// Add this to Awake method:
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    
        // ✅ FIX: Add scene loaded callback
        SceneManager.sceneLoaded += OnSceneLoaded;
    
        EnsureEventsHandler();
    }

// Don't forget to unsubscribe
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Start()
    {
        if (AutoStart)
        {
            CreateLobby();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[PNM] Scene loaded: {scene.name}");
    
        // Refresh all player UIs after scene load
        Invoke(nameof(RefreshAllPlayerUIs), 1f);
    }

    private void RefreshAllPlayerUIs()
    {
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            if (player != null)
            {
                player.RefreshPlayerUI();
            }
        }
    }
    private void EnsureEventsHandler()
    {
        if (PhotonEventsHandler.Instance == null)
        {
            if (eventsHandlerPrefab != null)
            {
                Instantiate(eventsHandlerPrefab);
            }
            else
            {
                // Create a new one if no prefab assigned
                var handlerObj = new GameObject("PhotonEventsHandler");
                handlerObj.AddComponent<PhotonEventsHandler>();
                DontDestroyOnLoad(handlerObj);
            }
        }
        Debug.Log($"[PNM] PhotonEventsHandler Instance: {PhotonEventsHandler.Instance != null}");
    }
    // ---------------------- CREATE ----------------------

    public void CreateLobby()
    {
        string code = UnityEngine.Random.Range(100000, 999999).ToString();
        LastSessionName = code;

        Debug.Log($"[PNM] Creating lobby: {code}");
        StartRunner(Fusion.GameMode.Host, code).Forget();

        HomeUIManager.Instance.UpdateSessionCode(code);
        OnLobbyCreated?.Invoke(code);
    }

    // ---------------------- PVP MATCHMAKING ----------------------
    
    public void StartPvPMatchmaking()
    {
        // Use a fixed session name for PvP matchmaking so players can find each other
        string pvpSession = "PvP_Global_Match";
        LastSessionName = pvpSession;
        
        Debug.Log($"[PNM] Starting PvP matchmaking: {pvpSession}");
        StartRunner(Fusion.GameMode.AutoHostOrClient, pvpSession).Forget();
    }

    // ---------------------- JOIN ----------------------

    public void JoinLobby(string code)
    {
        code = code.Trim();
        LastSessionName = code;

        Debug.Log($"[PNM] Attempt join lobby: {code}");
        StartRunner(Fusion.GameMode.Client, code).Forget();

        OnLobbyJoined?.Invoke();
    }

    // ---------------------- START RUNNER ----------------------
    private async Task StartRunner(Fusion.GameMode mode, string sessionName)
    {
        // Create runner if doesn't exist
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;
        }

        var sceneManager = GetComponent<NetworkSceneManagerDefault>() 
                           ?? gameObject.AddComponent<NetworkSceneManagerDefault>();

        var startArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager,
            PlayerCount = 2,
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "VISIBLE", 1 }
            }
        };

        Debug.Log($"[PNM] Starting runner as {mode} (Room={sessionName})...");
        var result = await _runner.StartGame(startArgs);

        if (result.Ok)
        {
            Debug.Log($"[PNM] Runner started successfully as {mode}. Room: {sessionName}");
        
            // Register callbacks
            if (PhotonEventsHandler.Instance != null)
            {
                _runner.AddCallbacks(PhotonEventsHandler.Instance);
            }
        
            // ✅ ADD THIS: Register the SimplePlayerSpawner
            var spawner = FindObjectOfType<PlayerSpawner>();
            if (spawner != null)
            {
                _runner.AddCallbacks(spawner);
                Debug.Log("[PNM] ✅ Registered Player Spawner");
            }
        }
        else
        {
            Debug.LogError($"[PNM] FAILED: {result.ShutdownReason}");
            Debug.LogError($"[PNM] Error Details: {result.ErrorMessage}");
            
            // Handle connection timeouts
            if (result.ShutdownReason.ToString().Contains("Timeout"))
            {
                Debug.Log("[PNM] Connection timeout - showing error to user");
                HandleConnectionError("Connection timeout. Please check your internet connection.");
            }
        }
    }

    // ---------------------- LEAVE ----------------------

    public void LeaveSession()
    {
        if (_runner != null)
        {
            Debug.Log("[PNM] Leaving session...");
            _ = _runner.Shutdown();
            Destroy(_runner);
            _runner = null;
        }
    }
    
    public void ForceCleanup()
    {
        Debug.Log("[PNM] Force cleanup - destroying all network components");
        
        if (_runner != null)
        {
            Destroy(_runner);
            _runner = null;
        }
        
        // Clean up any remaining network objects
        var networkObjects = FindObjectsOfType<NetworkObject>();
        foreach (var obj in networkObjects)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
    }
    
    public void HandleOpponentDisconnect()
    {
        Debug.Log("[PNM] Opponent disconnected - ending match");
        
        // Leave current session
        LeaveSession();
        
        // Show match over screen
        if (MatchOverManager.Instance != null)
        {
            MatchOverManager.Instance.ShowMatchOver("You Win!", "Opponent disconnected");
        }
    }
    
    private void HandleConnectionError(string errorMessage)
    {
        Debug.LogWarning($"[PNM] Connection error: {errorMessage}");
        
        // Go back to home and show error
        if (HomeUIManager.Instance != null)
        {
            HomeUIManager.Instance.ShowError(errorMessage);
            
            // Switch back to home panel
            GameObject loadingPanel = GameObject.Find("LoadingPanel");
            GameObject homePanel = GameObject.Find("HomePanel");
            
            if (loadingPanel != null && homePanel != null)
            {
                loadingPanel.SetActive(false);
                homePanel.SetActive(true);
            }
        }
    }
}

// ---------------------- FIRE AND FORGET ----------------------

static class TaskExtensions
{
    public static async void Forget(this Task t)
    {
        try { await t; } catch (Exception ex) { Debug.LogException(ex); }
    }
}
