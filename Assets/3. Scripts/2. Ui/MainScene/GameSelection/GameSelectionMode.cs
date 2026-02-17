using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameMode", menuName = "GameMode", order = 0)]
public class GameSelectionMode : ScriptableObject 
{
    public List<GameDetails> Gamelist = new List<GameDetails>();

    public Mode CurrentType;

    public void loadGameScene()
    {   
       switch(CurrentType)
        {
            case Mode.SinglePlayer_Type:
                HomeUIManager.Instance.OnClickCampaignButton();
            break;

            case Mode.MultiPlayer_Lobby_Type:
                HomeUIManager.Instance.OnClickPrivateLobbyButton();
            break;

            case Mode.Multiplayer_PVP_Type:
                HomeUIManager.Instance.OnClickPVPButton();
            break;

            case Mode.Scenario_Type:
                HomeUIManager.Instance.OnClickCampaignButton();
            break;
        }
        HomeUIManager.Instance.ShowPanel(PanelName.Faction);
    }
}

public enum Mode
{
    SinglePlayer_Type,
    MultiPlayer_Lobby_Type,
    Multiplayer_PVP_Type,
    Scenario_Type, 
    None
}

[System.Serializable]
public class GameDetails
{
    public Mode GameType;
    public string GameName;
    public Sprite GameIcon;
    public string SceneName;
}
