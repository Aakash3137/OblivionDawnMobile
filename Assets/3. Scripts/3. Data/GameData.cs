using UnityEngine;

public static class GameData
{
    public static Mode gameMode;
    public static Difficulty difficulty;
    public static PlayStyle playStyle;
    public static FactionName playerFaction;
    public static FactionName enemyFaction;
    public static int mapLevel;
    public static int totalLevels;

    //  for Stat data
    public static readonly int GameMaxDeckSize;
    public static readonly int GameMaxPopulation;
    public static int GameMaxObjectLevel;


    // should remove use of strings
    public static string GameModeType;
    public static string SelectedFactionName;
    public static MP_Faction SelectedMPFaction;

    // Addressable map assets
    public static GameObject loadedEnvironmentPrefab;
    public static Texture2D  loadedTileTexture;
    
    static GameData()
    {
        gameMode = Mode.None;
        difficulty = Difficulty.Easy;
        playStyle = PlayStyle.Aggressive;
        playerFaction = FactionName.Futuristic;
        enemyFaction = FactionName.Futuristic;

        mapLevel = 1;
        totalLevels = 15;

        GameMaxDeckSize = 8;
        GameMaxPopulation = 40;
        GameMaxObjectLevel = 20;
    }
}
