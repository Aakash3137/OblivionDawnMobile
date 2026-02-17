using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkGameUI : MonoBehaviour
{
    [Header("Panels and Popups")]
    public List<PanelInfo> panels;
    public List<PopupInfo> popups;
    [SerializeField] internal List<LobbyType> LobbyPanels;
    [SerializeField] internal TMP_InputField LobbyCodeInputField;
    [SerializeField] internal TMP_Text Player1NameText;
    [SerializeField] internal TMP_Text Player2NameText;
    [SerializeField] internal TMP_Text SessionCodeText;

    [Header("Player 1 Info")]
    [SerializeField] internal TextMeshProUGUI player1NameText;
    [SerializeField] internal TextMeshProUGUI player1RankText;
    [SerializeField] internal TextMeshProUGUI player1TileCount;
    
    [Header("Player 2 Info")] 
    [SerializeField] internal TextMeshProUGUI player2NameText;
    [SerializeField] internal TextMeshProUGUI player2RankText;
    [SerializeField] internal TextMeshProUGUI player2TileCount;
    
    internal Color playerColor = Color.green;
    internal Color enemyColor = Color.red;

    [SerializeField] internal GameObject LoadingPanel;
    
    [SerializeField] private GameSelectionMode CurrentMode;
    [SerializeField] internal Camera _MainCamera;
    private bool _cameraRotationApplied = false;

    private void Start()
    {
        Debug.Log("[PrivateLobbyUI] PrivateLobbyUI started, waiting for players...");
        
        // ✅ FIX: Wait longer for players to be properly loaded
        Invoke(nameof(RefreshPlayerInfo), 1f);
        InvokeRepeating(nameof(RefreshPlayerInfo), 1f, 2f);
        
        LoadingPanel.SetActive(true);
        Invoke(nameof(TurnOffLoadingPanel), 5f);
    }

    private void TurnOffLoadingPanel()
    {
        LoadingPanel.SetActive(false);
        ShowPanel(CurrentMode.CurrentType);
    }

#region UI Management
    public void ShowPanel(Mode panel)
    {
        foreach (var panelInfo in panels)
        {
            panelInfo.PanelObject.SetActive(false);
        }
        panels.Find(p => p.PanelName == panel)?.PanelObject.SetActive(true);
    }

    public void ShowPopup(PopupType popup)
    {
        foreach(var panelInfo in panels)        
        {
            panelInfo.PanelObject.SetActive(false);
        }
        popups.Find(p => p.PopupName == popup)?.PopupObject.SetActive(true);
    }

    internal void ShowLobby(LobbyName lobbyName)
    {
        foreach(LobbyType lobby in LobbyPanels)
        {
            lobby.PanelObject.SetActive(false);
        }
        LobbyPanels.Find(X => X.PanelName == lobbyName)?.PanelObject.SetActive(true);
    }
#endregion

    public void RefreshPlayerInfo()
    {
        try
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
           // Debug.Log($"[NetworkGameUI] Found {players.Length} NetworkPlayer objects in scene");

            NetworkPlayer localPlayer = players.FirstOrDefault(p => p.Object.HasInputAuthority);
            NetworkPlayer enemyPlayer = players.FirstOrDefault(p => !p.Object.HasInputAuthority);

            // Local player always on left (Player 1 UI)
            if (localPlayer != null && localPlayer.IsProfileSet)
            {
                SetupPlayerUI(localPlayer, player1NameText, player1RankText, player1TileCount, playerColor, true);
            }
            else
            {
                SetEmptyPlayerUI(player1NameText, player1RankText);
            }
            
            // Enemy player always on Right (Player 2 UI)
            if (enemyPlayer != null && enemyPlayer.IsProfileSet)
            {
                SetupPlayerUI(enemyPlayer, player2NameText, player2RankText, player2TileCount, enemyColor, false);
            }
            else
            {
                SetEmptyPlayerUI(player2NameText, player2RankText);
            }
        }
        catch (System.Exception ex)
        {
           // Debug.LogWarning($"[NetworkGameUI] Error: {ex.Message}");
        }
    }


    private void SetupPlayerUI(NetworkPlayer player, TextMeshProUGUI nameText, TextMeshProUGUI rankText,TextMeshProUGUI tileCount, Color color, bool isPlayer)
    {
        if (player != null && player.IsProfileSet)
        {
            nameText.text = player.GetDisplayName();
            nameText.color = color;
            rankText.text = $" {player.GetRank()}";
            if (isPlayer)
            {
                tileCount.text = $" {NetworkCubeGridManager.Instance.playerTileCount}";
                tileCount.color = color;
            }
            else
            {
                tileCount.text = $" {NetworkCubeGridManager.Instance.playerTileCount}";
                tileCount.color = color;
            }
        }
        else
        {
            SetEmptyPlayerUI(nameText, rankText);
        }
    }

    //player leave ui update
    internal void SetEmptyPlayerUI(TextMeshProUGUI nameText, TextMeshProUGUI rankText, TextMeshProUGUI tileCount = null)
    {
        nameText.text = "Waiting...";
        nameText.color = Color.white;
        rankText.text = "Rank -";
        tileCount.text = "Tiles -";
    }

   
    private void OnDestroy()
    {
        CancelInvoke(nameof(RefreshPlayerInfo));
    }
}

[System.Serializable]
public class PanelInfo
{
    public GameObject PanelObject;
    public Mode PanelName;
}

[System.Serializable]
public class PopupInfo
{
    public GameObject PopupObject;
    public PopupType PopupName;
}

public enum PopupType
{
    BuildPopup
}
public enum GameScenePanels
{
    GamePlay,
    PrivateLobby,
    PVP,
    MatchOver,
    Loading
}