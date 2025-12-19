using System;
using System.Collections;
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
    [SerializeField] private GameObject _uiLoginPanel;
    [SerializeField] private GameObject HomePanel;
    [SerializeField] private GameObject ProfilePanel;
    [SerializeField] private GameObject PrivateLobbyPanel;
    [SerializeField] private GameObject JoinLobbyPanel;
    [SerializeField] private GameObject PlayerJoinedPanel;
    [SerializeField] private GameObject LoadingPanel;
    [SerializeField] private GameObject PvpDisplayPanel;
    [SerializeField] CanvasGroup playerFactionPanel; //--------

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

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        SetupButtonListeners();
        playerFactionPanel.GetComponent<CanvasGroup>(); //--------

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
        // loginButton.onClick.AddListener(OnClickLoginButton);
        // signUpButton.onClick.AddListener(OnClickSignUpButton);
        // guestButton.onClick.AddListener(OnClickGuestLogin);

        // // Main menu buttons

        // shopButton.onClick.AddListener(OnClickShopButton);
        // upgradeButton.onClick.AddListener(OnClickUpgradeButton);

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
        ActivatePanel(HomePanel);
    }
    public void OnClickSignUpButton() { }
    public void OnClickGuestLogin() { }


    private void OnClickProfileButton()
    {
        ProfilePanel.SetActive(true);
        HomePanel.SetActive(false);
    }
    private void OnClickCampaignButton()
    {
        // SwitchPanel(HomePanel, LoadingPanel);
        // // TODO: Load Campaign Scene
        // StartCoroutine(LoadSceneAfterDelay(2));

        playerFactionPanel.alpha = 1;
        playerFactionPanel.interactable = true;
        playerFactionPanel.blocksRaycasts = true;

        GameData.GameModeType = "Campaign";
    }
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SinglePlayerScene");
    }

    private void OnClickPVPButton()
    {
        CustomGameMode.SetGameMode(GameModeType.PvP);
        SwitchPanel(HomePanel, LoadingPanel);

        // Start matchmaking and show PvP panel after delay
        Invoke(nameof(StartPvPAndShowPanel), 0.1f);

        GameData.GameModeType = "PvP";
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

    private void OnClickPrivateLobbyButton()
    {
        Debug.Log("[HomeUIManager] Private lobby button clicked");
        SwitchPanel(HomePanel, PrivateLobbyPanel);

        GameData.GameModeType = "PrivateLobby";
    }

    private void OnClickCreateLobbyButton()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        SwitchPanel(PrivateLobbyPanel, LoadingPanel);

        // Start lobby creation and show panel after delay
        Invoke(nameof(StartLobbyAndShowPanel), 0.1f);
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
        SwitchPanel(PrivateLobbyPanel, JoinLobbyPanel);
    }

    private void OnJoinButtonClicked()
    {
        CustomGameMode.SetGameMode(GameModeType.HostClient);
        SwitchPanel(JoinLobbyPanel, PlayerJoinedPanel);
        PhotonNetworkManager.Instance.JoinLobby(LobbyCodeInputField.text);
    }



    public void OnClickShopButton() { }
    public void OnClickUpgradeButton() { }

    #endregion

    #region Navigation Handlers

    private void OnPrivateLobbyBackButton()
    {
        SwitchPanel(PrivateLobbyPanel, HomePanel);
    }

    private void OnJoinLobbyBackButton()
    {
        SwitchPanel(JoinLobbyPanel, PrivateLobbyPanel);
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

    private void SwitchPanel(GameObject fromPanel, GameObject toPanel)
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

    #endregion
}
