using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PvPMatchUI : MonoBehaviour
{
    #region UI Elements
    
    [Header("Player Panels")]
    [SerializeField] private GameObject player1Panel;
    [SerializeField] private GameObject player2Panel;
    
    [Header("Player 1 Info")]
    [SerializeField] private TextMeshProUGUI player1Name;
    [SerializeField] private TextMeshProUGUI player1Rank;
    
    [Header("Player 2 Info")]
    [SerializeField] private TextMeshProUGUI player2Name;
    [SerializeField] private TextMeshProUGUI player2Rank;
    
    [Header("Match Status")]
    [SerializeField] private TextMeshProUGUI matchStatusText;
    
    #endregion
    
    #region Configuration
    
    [Header("Matchmaking Settings")]
    [SerializeField] private int rankTolerance = 10;
    [SerializeField] private float autoStartDelay = 2f;
    [SerializeField] private float pvpTimeout = 20f;
    
    #endregion
    
    #region Private Fields
    
    private bool _matchStarted = false;
    private bool _botMatchmakingStarted = false;
    private int _countdownTimer = 3;
    private bool _countdownActive = false;
    
    #endregion

    #region Unity Lifecycle
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void OnEnable()
    {
        // Restart UI when panel is reactivated
        if (_matchStarted || _botMatchmakingStarted)
        {
            Debug.Log("[PvPMatchUI] Panel reactivated - restarting UI");
            InitializeUI();
        }
    }
    
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeUI()
    {
        // Reset all flags for fresh start
        _matchStarted = false;
        _botMatchmakingStarted = false;
        _countdownActive = false;
        _countdownTimer = 3;
        
        RefreshPlayerInfo();
        InvokeRepeating(nameof(RefreshPlayerInfo), 0.1f, 0.2f);
        StartBotMatchmakingTimer();
    }
    
    private void StartBotMatchmakingTimer()
    {
        Invoke(nameof(OnPvPTimeout), pvpTimeout);
        Debug.Log($"[PvPMatchUI] PvP timeout timer started - {pvpTimeout} seconds");
    }
    
    #endregion

    #region Player Info Management
    
    public void RefreshPlayerInfo()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        
        HandlePlayer1Display(players);
        HandlePlayer2Display(players);
    }
    
    private void HandlePlayer1Display(NetworkPlayer[] players)
    {
        if (players.Length >= 1 && players[0].IsProfileSet)
        {
            ShowPlayer1Info(players[0]);
        }
    }
    
    private void HandlePlayer2Display(NetworkPlayer[] players)
    {
        if (players.Length >= 2 && players[1].IsProfileSet)
        {
            CancelBotMatchmaking();
            ShowPlayer2Info(players[1]);
            ProcessMatchmaking(players[0], players[1]);
        }
        else
        {
            ShowWaitingForOpponent();
        }
    }
    
    private void CancelBotMatchmaking()
    {
        if (!_botMatchmakingStarted)
        {
            CancelInvoke(nameof(OnPvPTimeout));
            Debug.Log("[PvPMatchUI] PvP timeout cancelled - player found");
        }
    }
    
    private void ProcessMatchmaking(NetworkPlayer player1, NetworkPlayer player2)
    {
        if (!_countdownActive)
        {
            if (AreRanksCompatible(player1, player2))
            {
                ShowMatchReady();
                StartAutoMatchTimer();
            }
            else
            {
                ShowRankMismatch(player1, player2);
            }
        }
    }
    
    #endregion

    #region UI Display Methods
    
    private void ShowPlayer1Info(NetworkPlayer player)
    {
        player1Panel?.SetActive(true);
        if (player1Name != null) player1Name.text = player.GetDisplayName();
        if (player1Rank != null) player1Rank.text = $"Rank: {player.GetRank()}";
    }
    
    private void ShowPlayer2Info(NetworkPlayer player)
    {
        player2Panel?.SetActive(true);
        if (player2Name != null) player2Name.text = player.GetDisplayName();
        if (player2Rank != null) player2Rank.text = $"Rank: {player.GetRank()}";
    }
    
    private void ShowWaitingForOpponent()
    {
        player2Panel?.SetActive(false);
        UpdateMatchStatus("Searching for opponent...");
    }
    
    private void ShowMatchReady()
    {
        UpdateMatchStatus("Match Ready! Starting in 3...");
    }
    
    private void ShowRankMismatch(NetworkPlayer player1, NetworkPlayer player2)
    {
        int rankDiff = Mathf.Abs(player1.GetRank() - player2.GetRank());
        UpdateMatchStatus($"Rank difference too high ({rankDiff}). Searching for better match...");
    }
    
    private void UpdateMatchStatus(string message)
    {
        if (matchStatusText != null) matchStatusText.text = message;
    }
    
    #endregion

    #region Matchmaking Logic
    
    private bool AreRanksCompatible(NetworkPlayer player1, NetworkPlayer player2)
    {
        int rankDifference = Mathf.Abs(player1.GetRank() - player2.GetRank());
        return rankDifference <= rankTolerance;
    }
    
    private void StartAutoMatchTimer()
    {
        if (!_matchStarted)
        {
            _matchStarted = true;
            _countdownActive = true;
            _countdownTimer = 3;
            InvokeRepeating(nameof(UpdateCountdown), 1f, 1f);
        }
    }
    
    private void UpdateCountdown()
    {
        if (_countdownTimer > 0)
        {
            UpdateMatchStatus($"Match starting in {_countdownTimer}...");
            _countdownTimer--;
        }
        else
        {
            CancelInvoke(nameof(UpdateCountdown));
            _countdownActive = false;
            StartPvPMatch();
        }
    }
    
    private void StartPvPMatch()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    #endregion
    
    #region Bot Matchmaking
    
    private void OnPvPTimeout()
    {
        if (!_matchStarted && !_botMatchmakingStarted)
        {
            Debug.Log("[PvPMatchUI] PvP timeout reached - calling timeout function");
            HandlePvPTimeout();
        }
    }
    
    private void HandlePvPTimeout()
    {
        UpdateMatchStatus("No opponent found. Returning to home...");
        Debug.Log("[PvPMatchUI] PvP timeout - returning to home page");
        
        // Go back to home page after 2 seconds
        Invoke(nameof(GoBackToHome), 2f);
    }
    
    private void GoBackToHome()
    {
        Debug.Log("[PvPMatchUI] Going back to home page");
        
        // Cancel any running timers
        CancelInvoke();
        
        // Complete network cleanup for fresh start
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.ForceCleanup();
        }
        
        // Use HomeUIManager to properly switch panels
        if (HomeUIManager.Instance != null)
        {
            HomeUIManager.Instance.SwitchToHomePanel();
        }
    }
    
    private void StartBotMatch()
    {
        _botMatchmakingStarted = true;
        UpdateMatchStatus("No opponent found. Starting match against bot...");
        
        Debug.Log("[PvPMatchUI] Initiating bot match");
        
        // Start bot match after short delay
        Invoke(nameof(LoadBotMatch), 2f);
    }
    
    private void LoadBotMatch()
    {
        Debug.Log("[PvPMatchUI] Loading bot match scene");
        // TODO: Load bot match scene or set bot flag
        //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    #endregion
}