using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonNetworkManager : MonoBehaviour
{
    public static PhotonNetworkManager Instance { get; private set; }

    public bool AutoStart = false;
    public string LastSessionName;

    private NetworkRunner _runner;
    public NetworkRunner Runner => _runner;

    public event Action<string> OnLobbyCreated;
    public event Action OnLobbyJoined;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (AutoStart)
        {
            CreateLobby();
        }
    }

    // ---------------------- CREATE ----------------------

    public void CreateLobby()
    {
        string code = UnityEngine.Random.Range(100000, 999999).ToString();
        LastSessionName = code;

        Debug.Log($"[PNM] Creating lobby: {code}");
        StartRunner(GameMode.Host, code).Forget();

        HomeUIManager.Instance.UpdateSessionCode(code);
        OnLobbyCreated?.Invoke(code);
    }

    // ---------------------- JOIN ----------------------

    public void JoinLobby(string code)
    {
        code = code.Trim();
        LastSessionName = code;

        Debug.Log($"[PNM] Attempt join lobby: {code}");
        StartRunner(GameMode.Client, code).Forget();

        OnLobbyJoined?.Invoke();
    }

    // ---------------------- START RUNNER ----------------------

    private async Task StartRunner(GameMode mode, string sessionName)
    {
        // Create runner if doesn't exist
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;
        }

        // Prevent duplicate SceneManager components
        var sceneManager = GetComponent<NetworkSceneManagerDefault>() 
                           ?? gameObject.AddComponent<NetworkSceneManagerDefault>();

        var startArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager,

            // 👇 This makes the session discoverable for JoinLobby
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
        }
        else
        {
            Debug.LogError($"[PNM] FAILED: {result.ShutdownReason}");

            if (mode == GameMode.Client && result.ShutdownReason == ShutdownReason.GameNotFound)
            {
                Debug.LogError("[PNM] ❌ Could NOT join room — check lobby code or network visibility.");
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
}

// ---------------------- FIRE AND FORGET ----------------------

static class TaskExtensions
{
    public static async void Forget(this Task t)
    {
        try { await t; } catch (Exception ex) { Debug.LogException(ex); }
    }
}
