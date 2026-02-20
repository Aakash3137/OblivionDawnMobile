using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    #region Singleton

    public static HomeUIManager Instance { get; private set; }

    #endregion

    #region UI Panels

    [Header("Panels")]
    [SerializeField] private List<PanelDetails> UIPanels = new List<PanelDetails>();
    [SerializeField] internal GameObject _uiLoginPanel;
    [SerializeField] internal GameObject HomePanel;
    [SerializeField] internal GameObject ProfilePanel;
    [SerializeField] private GameObject PrivateLobbyPanel;
    [SerializeField] private GameObject JoinLobbyPanel;
    [SerializeField] private GameObject PlayerJoinedPanel;
    [SerializeField] internal GameObject LoadingPanel;
    [SerializeField] private GameObject PvpDisplayPanel;
    //[SerializeField] CanvasGroup playerFactionPanel; //--------
    [SerializeField] FactionPanelScript playerFactionPanel;

    #endregion

    #region UI Buttons


    [Header("Login Panel Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button guestButton;


    [Header("Main Buttons")]
    [SerializeField] private Button ProfileButton;
    [SerializeField] private Button CampaignButton;
    [SerializeField] private Button PrivateLobbyButton;
    [SerializeField] private Button PVPButton;
    [SerializeField] private Button DecButton;


    [SerializeField] private Button shopButton;
    [SerializeField] private Button upgradeButton;

    [Header("Lobby Buttons")]
    [SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button OuterJoinLobbyButton;
    [SerializeField] private Button JoinLobbyButton;

    [Header("Navigation Buttons")]
    [SerializeField] private Button PrivateLobbyBackButton;
    [SerializeField] private Button JoinLobbyBackButton;

    #endregion

    #region UI Elements
    [Header("Play as Guest")]
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_Text userNameError;

    [Header("Join Panel Elements")]
    [SerializeField] private TMP_InputField LobbyCodeInputField;
    [SerializeField] private TextMeshProUGUI LobbyCodeErrorText;

    [Header("Player Info Elements")]
    [SerializeField] private TextMeshProUGUI Player1NameText;
    [SerializeField] private TextMeshProUGUI Player1RankText;
    [SerializeField] private TextMeshProUGUI Player2NameText;
    [SerializeField] private TextMeshProUGUI Player2RankText;
    [SerializeField] private TextMeshProUGUI SessionCodeText;

    #endregion


    #region Scriptable Objects
    [Header("Profile")]
    [SerializeField] private Userdata Profiledata;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnEnable()
    {
        if (Profiledata.GuestUser && Profiledata.UserName != "")
        {
            Debug.Log("Login");
            ShowPanel(PanelName.Home);
        }
        else
        {
            Debug.Log("GO Home with " + Profiledata.GuestUser);
            ShowPanel(PanelName.Login);
        }
    }

    private void Start()
    {
        SetupButtonListeners();
        //playerFactionPanel = playerFactionPanel.GetComponent<FactionPanelScript>().Layout; //--------

    }

    #endregion

    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupButtonListeners()
    {
        //Login Panel Listeners
        loginButton.onClick.AddListener(OnClickLoginButton);
        signUpButton.onClick.AddListener(OnClickSignUpButton);
        guestButton.onClick.AddListener(OnClickGuestLogin);

        // Main menu buttons
        shopButton.onClick.AddListener(OnClickShopButton);
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        ProfileButton.onClick.AddListener(OnClickProfileButton);
        CampaignButton.onClick.AddListener(OnClickCampaignButton);
        PrivateLobbyButton.onClick.AddListener(OnClickPrivateLobbyButton);
        PVPButton.onClick.AddListener(OnClickPVPButton);

        // Lobby buttons
        CreateLobbyButton.onClick.AddListener(OnClickCreateLobbyButton);
        OuterJoinLobbyButton.onClick.AddListener(OpenJoinLobbyPanel);
        JoinLobbyButton.onClick.AddListener(OnJoinButtonClicked);

        // Navigation buttons
        PrivateLobbyBackButton.onClick.AddListener(OnPrivateLobbyBackButton);
        JoinLobbyBackButton.onClick.AddListener(OnJoinLobbyBackButton);
    }

    #endregion

    #region Button Event Handlers


    public void OnClickLoginButton()
    {
        // ActivatePanel(HomePanel);
        ShowPanel(PanelName.Home);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    public void OnClickSignUpButton()
    {
        ShowPanel(PanelName.Signup);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    public void OnClickGuestLogin()
    {
        if (userNameInput.text == "" || userNameInput.text == null)
        {
            userNameError.text = "Please Enter User Name!!!";
            userNameError.color = Color.red;
            StartCoroutine(DisableErrortext());
            return;
        }
        string username = userNameInput.text.Trim();
        Profiledata.UserName = username;
        Profiledata.GuestUser = true;
        Debug.Log("Guest Username: " + username);
        ShowPanel(PanelName.Home);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private IEnumerator DisableErrortext()
    {
        yield return new WaitForSeconds(5f);
        userNameError.gameObject.SetActive(false);
    }

    private void OnClickProfileButton()
    {
        //ProfilePanel.SetActive(true);
        //HomePanel.SetActive(false);
        ShowPanel(PanelName.Profile);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    public void OnClickCampaignButton()
    {
        // SwitchPanel(HomePanel, LoadingPanel);
        // // TODO: Load Campaign Scene
        // StartCoroutine(LoadSceneAfterDelay(2));
        GameData.GameModeType = "Campaign";
        LoadFactionPanel();
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SinglePlayerScene");
    }

    public void OnClickPVPButton()
    {
        CustomGameMode.SetGameMode(GameModeType.PvP);
        GameData.GameModeType = "PVP";
        //SwitchPanel(HomePanel, LoadingPanel);

        LoadFactionPanel();
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void StartPvPAndShowPanel()
    {
        PhotonNetworkManager.Instance.StartPvPMatchmaking();

        // Show PvP panel after 3 seconds to allow player spawning
        Invoke(nameof(ShowPvPPanel), 3f);
    }

    private void ShowPvPPanel()
    {
        SwitchPanel(LoadingPanel, PvpDisplayPanel);
    }

    public void OnClickPrivateLobbyButton()
    {
        GameData.GameModeType = "Lobby";
        //SwitchPanel(HomePanel, PrivateLobbyPanel);
        LoadFactionPanel();
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void OnClickCreateLobbyButton()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        //SwitchPanel(PrivateLobbyPanel, LoadingPanel);

        LoadingPanel.SetActive(true);
        //PrivateLobbyPanel is Parent of PlayerJoinedPanel so disable PrivateLobbyPanel will disable PlayerJoinedPanel

        // Start lobby creation and show panel after delay
        Invoke(nameof(StartLobbyAndShowPanel), 0.1f);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void StartLobbyAndShowPanel()
    {
        PhotonNetworkManager.Instance.CreateLobby();

        // Show lobby panel after 3 seconds (unlimited waiting after that)
        Invoke(nameof(ShowLobbyPanel), 3f);
    }

    private void ShowLobbyPanel()
    {
        SwitchPanel(LoadingPanel, PlayerJoinedPanel);
    }

    private void OpenJoinLobbyPanel()
    {
        //SwitchPanel(PrivateLobbyPanel, JoinLobbyPanel);

        //PrivateLobbyPanel is Parent of JoinLobbyPanel so disable PrivateLobbyPanel will disable JoinLobbyPanel
        JoinLobbyPanel.SetActive(true);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void OnJoinButtonClicked()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        SwitchPanel(JoinLobbyPanel, PlayerJoinedPanel);
        PhotonNetworkManager.Instance.JoinLobby(LobbyCodeInputField.text);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void LoadFactionPanel()
    {
        // playerFactionPanel.Layout.alpha = 1;
        // playerFactionPanel.Layout.interactable = true;
        // playerFactionPanel.Layout.blocksRaycasts = true;
        ShowPanel(PanelName.Faction);
    }

    internal void OnFactionClicked()
    {

        if (GameData.GameModeType == "PVP")
        {
            Invoke(nameof(StartPvPAndShowPanel), 0.1f);
        }
        else if (GameData.GameModeType == "Lobby")
        {
            //
            ShowPanel(PanelName.Home);
            SwitchPanel(playerFactionPanel.gameObject, PrivateLobbyPanel);
            //PrivateLobbyPanel.SetActive(true);    
        }
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    public void OnClickShopButton()
    {
        ShowPanel(PanelName.Shop);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    public void OnClickUpgradeButton()
    {
        ShowPanel(PanelName.Upgrade);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    #endregion

    #region Navigation Handlers

    private void OnPrivateLobbyBackButton()
    {
        SwitchPanel(PrivateLobbyPanel, HomePanel);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void OnJoinLobbyBackButton()
    {
        SwitchPanel(JoinLobbyPanel, PrivateLobbyPanel);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    #endregion

    #region Public API

    public void UpdateSessionCode(string sessionCode)
    {
        SessionCodeText.text = $"Session code: {sessionCode}";
    }

    public void ShowError(string error)
    {
        LobbyCodeErrorText.text = error;
    }

    public void SwitchToHomePanel()
    {
        // Cancel any pending PvP panel shows
        CancelInvoke(nameof(ShowPvPPanel));

        // Turn off all other panels and show home panel
        PvpDisplayPanel?.SetActive(false);
        LoadingPanel?.SetActive(false);
        PrivateLobbyPanel?.SetActive(false);
        JoinLobbyPanel?.SetActive(false);
        PlayerJoinedPanel?.SetActive(false);

        HomePanel?.SetActive(true);
        Debug.Log("[HomeUIManager] Switched to home panel");
    }

    #endregion

    #region Helper Methods

    public void SwitchPanel(GameObject fromPanel, GameObject toPanel)
    {
        if (fromPanel != null) fromPanel.SetActive(false);
        if (toPanel != null) toPanel.SetActive(true);
    }


    private void DisableAllPanels()
    {
        _uiLoginPanel.SetActive(false);
        HomePanel.SetActive(false);
        PrivateLobbyPanel.SetActive(false);
        PvpDisplayPanel.SetActive(false);
        LoadingPanel.SetActive(false);
    }
    private void ActivatePanel(GameObject panel, GameObject previousPanel = null)
    {
        DisableAllPanels();
        panel.SetActive(true);
        if (previousPanel != null)
            previousPanel.SetActive(true);
    }

    public void ShowPanel(PanelName TargetPanel)
    {

        Debug.Log($"Target Panel: {TargetPanel}");
        foreach (PanelDetails panel in UIPanels)
        {
            panel.PanelObject.SetActive(false);
        }
        PanelDetails selected = UIPanels.Find(x => x.panelName == TargetPanel);

        if (selected != null)
        {
            selected.PanelObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Panel not found: {TargetPanel}");
        }
    }

    #endregion
}

public enum PanelName
{
    Login,
    Signup,
    Faction,
    Home,
    Deck,
    Lobby,
    PVP,
    Profile,
    Shop,
    Upgrade,
    Rewards,
    Friends,
    Message,
    Ranking,
    HeroJourney
}

[System.Serializable]
public class PanelDetails
{
    public PanelName panelName;
    public GameObject PanelObject;
}
