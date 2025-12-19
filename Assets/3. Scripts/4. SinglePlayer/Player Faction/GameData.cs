using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    public static GameObject SelectedMainBuildingPrefab;
    public static string SelectedFactionName;
    public static string GameModeType;

    // Store all faction prefabs so we can pick a random enemy later
    public static List<GameObject> AllFactionPrefabs = new List<GameObject>();
}
