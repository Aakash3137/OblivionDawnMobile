// using UnityEngine;
// using System.Collections.Generic;

// public static class GameData
// {
//     // public static FactionData SelectedFaction;
//     // public static FactionData EnemyFaction;
//     public static GameObject SelectedMainBuildingPrefab;

//     public static FactionPrefabStore PrefabStore; // assign once (e.g., via a scene provider)

//     public static string SelectedFactionName;
//     public static string GameModeType;

//     // Store all faction prefabs so we can pick a random enemy later
//     public static List<GameObject> AllFactionPrefabs = new List<GameObject>();
// }




using UnityEngine;

public static class GameData
{
    // Reference to the single ScriptableObject asset that holds all factions
    public static AllFactionsData AllFactionsData;

    // Which faction the player picked (Past, Present, Future, Monster)
    public static FactionName SelectedFaction;

    // Which faction the enemy is (set at runtime, e.g. random)
    public static FactionName EnemyFaction;

    // Optional: store the selected main building prefab if needed
    public static GameObject SelectedMainBuildingPrefab;

    // Game mode info
    public static string GameModeType;

    public static string SelectedFactionName;
    public static  MP_Faction SelectedMPFaction;
}
