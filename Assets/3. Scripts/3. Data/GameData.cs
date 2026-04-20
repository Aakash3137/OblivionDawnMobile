public static class GameData
{
    public static Mode gameMode = Mode.None;
    public static FactionName playerFaction = FactionName.Futuristic;
    public static FactionName enemyFaction;
    public static int mapLevel = 1;
    public static MapLevelDataSO mapLevelData = null;

    //  for Stat data
    public static readonly int GameMaxDeckSize = 8;
    public static readonly int GameMaxPopulation = 40;
    public static int GameMaxObjectLevel = 20;


    // should remove use of strings
    public static string GameModeType;
    public static string SelectedFactionName;
    public static MP_Faction SelectedMPFaction;
}
