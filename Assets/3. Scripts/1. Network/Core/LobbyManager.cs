using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    [Header("Game Scene Name")]
    public string GameSceneName = "GameScene";

    public float CountdownTime = 5f;
    private NetworkRunner _runner;
    private bool _countdownStarted = false;

    [SerializeField] private NetworkGameUI GameUI;
    [SerializeField] private Button CretaeLobbyBtn, outerJoinLobbyBtn, joinLobbyBtn;

    private void Start()
    {
        GameUI.ShowLobby(LobbyName.PrivateLobby);
        CretaeLobbyBtn.onClick.AddListener(OnClickCreateLobbyButton);
        outerJoinLobbyBtn.onClick.AddListener(OpenJoinOuterLobbyPanel);
        joinLobbyBtn.onClick.AddListener(OnJoinInnerButtonClicked);
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
            _runner.LoadScene(SceneRef.FromIndex(sceneIndex));
            StartCoroutine(UnloadMainSceneAfterLoad());
        }
        else
        {
            Debug.LogError($"[LobbyManager] Cannot load scene: {GameSceneName} not found in build settings!");
        }
    }

    private IEnumerator UnloadMainSceneAfterLoad()
    {
        yield return new WaitForSeconds(2f);
        Scene mainScene = SceneManager.GetSceneByName("MainScene");
        if (mainScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(mainScene);
            Debug.Log("[LobbyManager] MainScene unloaded");
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

    #region  GameSceneFixup
      public void OnClickPVPButton()
    {
        CustomGameMode.SetGameMode(GameModeType.PvP);
        GameData.GameModeType = "PVP";
        GameUI.ShowPanel(Mode.Multiplayer_PVP_Type);
    }

   
   

    public void OnClickPrivateLobbyButton()
    {
        GameData.GameModeType = "Lobby";
        //SwitchPanel(HomePanel, PrivateLobbyPanel);
        //LoadFactionPanel();

        GameUI.ShowPanel(Mode.MultiPlayer_Lobby_Type);
        GameUI.ShowLobby(LobbyName.PrivateLobby);
    }

    private void OnClickCreateLobbyButton()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        
        GameUI.ShowPanel(Mode.None);           
        Invoke(nameof(StartLobbyAndShowPanel), 0.1f);
    }

    private void StartLobbyAndShowPanel()
    {
        PhotonNetworkManager.Instance.CreateLobby(GameUI);

        GameUI.ShowLobby(LobbyName.PlayerJoined);
        // GameUI._MainCamera.gameObject.SetActive(false);
        // Show lobby panel after 3 seconds (unlimited waiting after that)
        Invoke(nameof(ShowLobbyPanel), 3f);
    }

    private void ShowLobbyPanel()
    {
        GameUI._MainCamera.gameObject.SetActive(false);
        GameUI.ShowPanel(Mode.MultiPlayer_Lobby_Type);
    }

    private void OpenJoinOuterLobbyPanel()
    {
        //PrivateLobbyPanel is Parent of JoinLobbyPanel so disable PrivateLobbyPanel will disable JoinLobbyPanel
        GameUI.ShowLobby(LobbyName.JoinLobby); /*  */
    }

    private void OnJoinInnerButtonClicked()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        GameUI._MainCamera.gameObject.SetActive(false);
        GameUI.ShowLobby(LobbyName.PlayerJoined);
        PhotonNetworkManager.Instance.JoinLobby(GameUI.LobbyCodeInputField.text);
    }
    #endregion
}


[System.Serializable]
public class LobbyType
{
    public LobbyName PanelName;
    public GameObject PanelObject;
}

public enum LobbyName
{
    PrivateLobby,
    JoinLobby,
    PlayerJoined
}