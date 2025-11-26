using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchOverManager : MonoBehaviour
{
    public static MatchOverManager Instance { get; private set; }
    
    [Header("Match Over Panel")]
    [SerializeField] private GameObject matchOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        SetupButtons();
        // Delay event subscription to ensure PhotonEventsHandler is ready
        Invoke(nameof(SubscribeToEvents), 01f);
    }
    
    private void SetupButtons()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgain);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    
    private void SubscribeToEvents()
    {
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerLeftEvent += HandlePlayerLeft;
            Debug.Log("[MatchOverManager] Successfully subscribed to events");
        }
        else
        {
            Debug.LogWarning("[MatchOverManager] PhotonEventsHandler not ready, retrying...");
            Invoke(nameof(SubscribeToEvents), 1f);
        }
    }
    
    private void HandlePlayerLeft(PlayerRef player)
    {
        // Only handle in PvP mode
        if (CustomGameMode.CurrentGameMode == GameModeType.PvP)
        {
            Debug.Log($"[MatchOverManager] Player left in PvP: {player}");
            ShowMatchOver("You Win!\n", "Opponent disconnected");
        }
    }
    
    public void ShowMatchOver(string result, string reason = "")
    {
        Debug.Log($"[MatchOverManager] Showing match over: {result}");
        
        // Load main scene first
        SceneManager.LoadScene("MainScene");
        
        // Show match over panel after scene loads
        Invoke(nameof(ShowMatchOverPanel), 1f);
        
        // Set result text
        if (resultText != null)
        {
            resultText.text = string.IsNullOrEmpty(reason) ? result : $"{result}\n{reason}";
        }
    }
    
    private void ShowMatchOverPanel()
    {
        if (matchOverPanel != null)
        {
            matchOverPanel.SetActive(true);
        }
    }
    
    private void OnPlayAgain()
    {
        Debug.Log("[MatchOverManager] Play Again clicked");
        
        // Hide match over panel
        if (matchOverPanel != null)
            matchOverPanel.SetActive(false);
            
        // Start new PvP match
        if (HomeUIManager.Instance != null)
        {
            CustomGameMode.SetGameMode(GameModeType.PvP);
            PhotonNetworkManager.Instance.StartPvPMatchmaking();
        }
    }
    
    private void OnMainMenu()
    {
        Debug.Log("[MatchOverManager] Main Menu clicked");
        
        // Hide match over panel
        if (matchOverPanel != null)
            matchOverPanel.SetActive(false);
            
        // Show home panel
        if (HomeUIManager.Instance != null)
        {
            GameObject homePanel = GameObject.Find("HomePanel");
            if (homePanel != null)
                homePanel.SetActive(true);
        }
    }
    
    private void OnDestroy()
    {
        CancelInvoke(); // Cancel any pending invokes
        
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerLeftEvent -= HandlePlayerLeft;
        }
    }
}