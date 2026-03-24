using UnityEngine;

public static class GameData
{
    public static Mode gameMode = Mode.None;
    public static FactionName playerFaction = FactionName.Futuristic;
    public static FactionName enemyFaction;


    // Game mode info
    public static string GameModeType;


    public static string SelectedFactionName;
    public static MP_Faction SelectedMPFaction;
}
