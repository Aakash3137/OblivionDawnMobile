using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class HomeUIManager : MonoBehaviour
{
    #region Variables
    [Header("Panel Reference")]
    [SerializeField] internal GameObject HomePanel;
    [SerializeField] internal GameObject PrivateLobbyPanel;
    [SerializeField] internal GameObject JoinLobbyPanel;
    [SerializeField] internal GameObject PlayerJoinedPanel;
    [SerializeField] internal GameObject LoadingPanel;
    
    [Header("Button Reference")]
    [SerializeField] internal Button CampaignButton;
    [SerializeField] internal Button PrivateLobbyButton;
    [SerializeField] internal Button PVPButton;
    
    [SerializeField] internal Button CreateLobbyButton;
    [SerializeField] internal Button OuterJoinLobbyButton;
    
    [SerializeField] internal Button PrivateLobbyBackButton;
    [SerializeField] internal Button JoinLobbyBackButton;
    
    [Header("JoinPanel")]
    [SerializeField] internal Button JoinLobbyButton;
    [SerializeField] internal TMP_InputField LobbyCodeInputField;
    
    [Header("PlayerJoinedPanel")]
    [SerializeField] internal TextMeshProUGUI Player1NameText;
    [SerializeField] internal TextMeshProUGUI Player1RankText;
    [SerializeField] internal TextMeshProUGUI Player2NameText;
    [SerializeField] internal TextMeshProUGUI Player2RankText;
    [SerializeField] internal TextMeshProUGUI SessionCodeText;

    public static HomeUIManager Instance;

    #endregion

    #region Unity Method
    private void Awake()
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

    private void Start()
    {
        CampaignButton.onClick.AddListener(OnClickCampaignButton);
        PrivateLobbyButton.onClick.AddListener(OnClickPrivateLobbyButton);
        PVPButton.onClick.AddListener(OnClickPVPButton);
        
        CreateLobbyButton.onClick.AddListener(OnClickCreateLobbyButton);
        OuterJoinLobbyButton.onClick.AddListener(OpenJoinLobbyPanel);
        JoinLobbyButton.onClick.AddListener(OnJoinButtonClicked);
        
        PrivateLobbyBackButton.onClick.AddListener(OnPrivateLobbyBackButton);
        JoinLobbyBackButton.onClick.AddListener(OnJoinLobbyBackButton);
    }
    
    #endregion

    #region Button Listeners
    private void OnClickCampaignButton()
    {
        HomePanel.SetActive(false);
        LoadingPanel.SetActive(true);
        
        // Load Campaign Scene
    }
    
    private void OnClickPVPButton()
    {
        HomePanel.SetActive(false);
        LoadingPanel.SetActive(true);

        // PVP Logic
    }
    
    private void OnClickPrivateLobbyButton()
    {
        HomePanel.SetActive(false);
        PrivateLobbyPanel.SetActive(true);
    }
    
    private void OnClickCreateLobbyButton()
    {
        PrivateLobbyPanel.SetActive(false);
        PlayerJoinedPanel.SetActive(true);
    
        // Set player 1 info (lobby creator)
        string playerName = RandomNameGenerator.GetRandomName();
        Player1NameText.text = playerName;
        Player1RankText.text = "Rank: 1";
    
        // Clear player 2 info
        Player2NameText.text = "Waiting...";
        Player2RankText.text = "";
    
        PhotonNetworkManager.Instance.CreateLobby();
    }

    public void UpdateSessionCode(string sessionCode)
    {
        SessionCodeText.text = $"Session: {sessionCode}";
    }

    public void UpdatePlayer2Info()
    {
        string playerName = RandomNameGenerator.GetRandomName();
        Player2NameText.text = playerName;
        Player2RankText.text = "Rank: 1";
    }

    private void OpenJoinLobbyPanel()
    {
        PrivateLobbyPanel.SetActive(false);
        JoinLobbyPanel.SetActive(true);
    }
    
    private void OnJoinButtonClicked()
    {
        JoinLobbyPanel.SetActive(false);
        PlayerJoinedPanel.SetActive(true);
        
        PhotonNetworkManager.Instance.JoinLobby(LobbyCodeInputField.text);
        UpdatePlayer2Info();
    }

    #endregion

    #region BackButton
    
    private void OnPrivateLobbyBackButton()
    {
        PrivateLobbyPanel.SetActive(false);
        HomePanel.SetActive(true);
    }
    
    
    private void OnJoinLobbyBackButton()
    {
        JoinLobbyPanel.SetActive(false);
        PrivateLobbyPanel.SetActive(true);
    }
     

    #endregion
}
