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
    [SerializeField] private float botMatchmakingTimeout = 20f;
    
    #endregion
    
    #region Private Fields
    
    private bool _matchStarted = false;
    private bool _botMatchmakingStarted = false;
    
    #endregion

    #region Unity Lifecycle
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeUI()
    {
        RefreshPlayerInfo();
        InvokeRepeating(nameof(RefreshPlayerInfo), 0.5f, 0.5f);
        StartBotMatchmakingTimer();
    }
    
    private void StartBotMatchmakingTimer()
    {
        Invoke(nameof(CheckForBotMatchmaking), botMatchmakingTimeout);
        Debug.Log($"[PvPMatchUI] Bot matchmaking timer started - {botMatchmakingTimeout} seconds");
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
            CancelInvoke(nameof(CheckForBotMatchmaking));
            Debug.Log("[PvPMatchUI] Bot matchmaking cancelled - player found");
        }
    }
    
    private void ProcessMatchmaking(NetworkPlayer player1, NetworkPlayer player2)
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
        UpdateMatchStatus("Match Ready! Starting in 2 seconds...");
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
            Invoke(nameof(StartPvPMatch), autoStartDelay);
        }
    }
    
    private void StartPvPMatch()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    #endregion
    
    #region Bot Matchmaking
    
    private void CheckForBotMatchmaking()
    {
        if (!_matchStarted && !_botMatchmakingStarted)
        {
            var players = FindObjectsOfType<NetworkPlayer>();
            
            if (players.Length == 1 && players[0].IsProfileSet)
            {
                Debug.Log("[PvPMatchUI] No suitable opponent found - starting bot match");
                StartBotMatch();
            }
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