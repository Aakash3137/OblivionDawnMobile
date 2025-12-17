using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Game Scene Name")]
    public string GameSceneName = "GameScene";

    public float CountdownTime = 5f;
    
    private NetworkRunner _runner;
    private bool _countdownStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _runner = PhotonNetworkManager.Instance?.Runner;
    }

    private void Update()
    {
        if (_runner == null) 
        {
            _runner = PhotonNetworkManager.Instance?.Runner;
            return;
        }

        CheckPlayerCountDirectly();
    }

    private void CheckPlayerCountDirectly()
    {
        if (_countdownStarted) return;
        
        var activePlayers = _runner.ActivePlayers.ToList();
        var networkPlayers = FindObjectsOfType<NetworkPlayer>();
        
        int activePlayerCount = activePlayers.Count;
        int networkPlayerCount = networkPlayers.Length;

        // Check both conditions - we need at least 2 NetworkPlayer objects AND 2 active players
        if (networkPlayerCount >= 2 && activePlayerCount >= 2 && _runner.IsServer)
        {
            Debug.Log("[LobbyManager] ✅ Starting countdown to game scene!");
            _countdownStarted = true;
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        Debug.Log($"[LobbyManager] Match starts in {CountdownTime} seconds...");
        
        // Update UI with countdown if needed
        for (int i = (int)CountdownTime; i > 0; i--)
        {
            Debug.Log($"[LobbyManager] Starting in {i}...");
            yield return new WaitForSeconds(1f);
        }

        if (_runner != null && _runner.IsServer)
        {
            Debug.Log("[LobbyManager] 🚀 Loading game scene...");
            
            // ✅ CRITICAL FIX: Don't destroy network objects when loading scene
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        int sceneIndex = GetSceneBuildIndex(GameSceneName);
        if (sceneIndex >= 0)
        {
            // ✅ CRITICAL FIX: Use LoadSceneMode.Single but preserve network objects
            _runner.LoadScene(SceneRef.FromIndex(sceneIndex), 
                new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Single));
        }
        else
        {
            Debug.LogError($"[LobbyManager] Cannot load scene: {GameSceneName} not found in build settings!");
        }
    }

    private int GetSceneBuildIndex(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneName)
                return i;
        }
        return -1;
    }

    private int GetActivePlayerCount()
    {
        return _runner.ActivePlayers.Count();
    }
}