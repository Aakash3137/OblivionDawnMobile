using UnityEngine;
using Fusion;
using System.Collections.Generic;

[System.Serializable]
public struct BuildRequestData
{
    public string tileName;
    public string buildingName;
    public int ownerSide;
    public string factionName;
}
public class NetworkBuildingManager : NetworkBehaviour
{
    public static NetworkBuildingManager Instance;

    [Header("Main Building Settings")]
    public string mainBuildingName = "MainBuilding";
    private bool mainBuildingsSpawned = false;
    private float spawnCheckDelay = 1f;
    
    [Header("Faction Manager")]
    public MP_FactionManager factionManager;

    private void Awake()
    {
        Instance = this;
        NetworkEventCore.AddListener(EventCode.BuildRequest, OnBuildRequestReceived);
        
        if (factionManager == null)
            factionManager = FindObjectOfType<MP_FactionManager>();
    }

    public override void Spawned()
    {
        if (Runner.IsServer && !mainBuildingsSpawned)
        {
            Invoke(nameof(SpawnMainBuildings), 2f);
        }
    }

    private void SpawnMainBuildings()
    {
        if (mainBuildingsSpawned) return;
        mainBuildingsSpawned = true;

        NetworkPlayer[] allPlayers = FindObjectsOfType<NetworkPlayer>();
        
        if (allPlayers.Length < 2)
        {
            Debug.LogWarning($"[NBM] Not enough players to spawn main buildings. Count: {allPlayers.Length}");
            // Try again later if players haven't connected yet
            Invoke(nameof(SpawnMainBuildings), 1f);
            return;
        }

        NetworkPlayer player0 = null;
        NetworkPlayer player1 = null;

        foreach (var player in allPlayers)
        {
            if (player.SpawnId == 0) player0 = player;
            else if (player.SpawnId == 1) player1 = player;
        }

        NetworkTile tile1 = FindTileByCoord(4, 4);
        NetworkTile tile2 = FindTileByCoord(12, 12);

        if (tile1 != null && player0 != null)
        {
            Debug.Log($"[NBM] Main building spawned at (4,4) for Player SpawnId 0 faction name is: " + player0.FactionName);
            SpawnFactionMainBuilding(tile1, player0.Object.InputAuthority, NetworkSide.Player, player0.FactionName.ToString());
            Debug.Log($"[NBM] Main building spawned at (4,4) for Player SpawnId 0");
        }

        if (tile2 != null && player1 != null)
        {
            Debug.Log($"[NBM] Main building spawned at (12,12) for Player SpawnId 1 faction name is: " + player1.FactionName);
            SpawnFactionMainBuilding(tile2, player1.Object.InputAuthority, NetworkSide.Player, player1.FactionName.ToString());
            Debug.Log($"[NBM] Main building spawned at (12,12) for Player SpawnId 1");
        }
    }
    
    private void SpawnFactionMainBuilding(NetworkTile tile, PlayerRef owner, NetworkSide ownerSide,  string factionName)
    {
        if (!Runner.IsServer) return;

        MP_Faction faction = factionManager.GetFactionByName(factionName);
        if (faction == null)
        {
            Debug.LogError($"[NBM] Faction not found: {factionName}");
            return;
        }

        GameObject mainBuildingPrefab = faction.mainBuildingPrefab;
        if (mainBuildingPrefab == null)
        {
            Debug.LogError($"[NBM] No main building prefab for faction: {factionName}");
            return;
        }

        Vector3 pos = tile.transform.position + Vector3.up;

        var spawnedObj = Runner.Spawn(mainBuildingPrefab, pos, Quaternion.identity, owner, (runner, obj) =>
        {
            var building = obj.GetComponent<NetworkBuilding>();
            if (building != null)
                building.OwnerSide = ownerSide;
        });

        if (spawnedObj != null)
        {
            tile.OccupyTile();
            NetworkCubeGridManager.Instance?.UpdateTileLists();
            Debug.Log($"[NBM] ✓ Faction main building spawned for player {owner} ({factionName})");
        }
        /*if (!Runner.IsServer) return;
        
        GameObject mainBuildingPrefab = GetMainBuildingPrefab();
        if (mainBuildingPrefab == null)
        {
            Debug.LogError($"[NBM] No main building prefab found for faction: {GameData.SelectedFactionName}");
            return;
        }

        Vector3 pos = tile.transform.position + Vector3.up;
        var spawnedObj = Runner.Spawn(mainBuildingPrefab, pos, Quaternion.identity, owner, (runner, obj) =>
        {
            var building = obj.GetComponent<NetworkBuilding>();
            if (building != null)
            {
                building.OwnerSide = ownerSide;
            }
        });
        
        if (spawnedObj != null)
        {
            tile.OccupyTile();
            NetworkCubeGridManager.Instance?.UpdateTileLists();
            Debug.Log($"[NBM] ✓ Faction main building spawned for player {owner}");
        }*/
    }
    
    private GameObject GetMainBuildingPrefab()
    {
        if (GameData.SelectedMPFaction != null && GameData.SelectedMPFaction.mainBuildingPrefab != null)
        {
            return GameData.SelectedMPFaction.mainBuildingPrefab;
        }
        
        if (factionManager != null && factionManager.allFactions != null)
        {
            foreach (var faction in factionManager.allFactions)
            {
                if (faction.factionName == GameData.SelectedFactionName)
                {
                    return faction.mainBuildingPrefab;
                }
            }
        }
        
        return null;
    }

    // ---------------- CLIENT → HOST ----------------
    public void RequestBuild(NetworkTile tile, string building)
    {
        if (tile == null) return;
        
        
        BuildRequestData data = new BuildRequestData
        {
            tileName = tile.name,
            buildingName = building,
            ownerSide = (int)tile.CurrentVisualOwner,
            factionName = GameData.SelectedFactionName
        };
        
        if (Runner.IsServer)
        {
            Debug.Log($"[NBM] Host clicked tile {tile.name}, processing locally.");
            ProcessBuildRequest(data, Runner.LocalPlayer);
        }
        else
        {
            Debug.Log($"[NBM] Client clicked tile {tile.name}, sending request to host.");
            NetworkEventCore.RaiseEvent(EventCode.BuildRequest, data, NetworkEventTargets.HostOnly);
        }
    }

    // ---------------- HOST RECEIVES ----------------
    private void OnBuildRequestReceived(string json)
    {
        if (!Runner.IsServer) return;

        BuildRequestData data = JsonUtility.FromJson<BuildRequestData>(json);
        Debug.Log($"[NBM] Host received build request for tile {data.tileName}, building: {data.buildingName}");
        Debug.Log($"[NBM] Host received build request for factionName: {data.factionName}");
        PlayerRef clientPlayer = NetworkEventCore.LastEventSender;
        ProcessBuildRequest(data, clientPlayer);
    }

    // ---------------- SPAWN LOGIC ----------------
    private void ProcessBuildRequest(BuildRequestData data, PlayerRef requester)
    {
        NetworkTile tile = FindTileByName(data.tileName);
        if (tile == null)
        {
            Debug.LogError($"[NBM] Tile {data.tileName} not found!");
            return;
        }

        if (tile.IsOccupied)
        {
            Debug.LogWarning($"[NBM] Tile {tile.name} is already occupied!");
            return;
        }

        NetworkSide ownerSide = (NetworkSide)data.ownerSide;
        Debug.Log($"[NBM] Host spawning building {data.buildingName} for player {requester}");
        Debug.Log("Faction name in Process Build request :"+ data.factionName);
        SpawnBuilding(tile, data.buildingName, ownerSide, requester, data.factionName);
    }

    private NetworkTile FindTileByName(string tileName)
    {
        NetworkTile[] allTiles = FindObjectsOfType<NetworkTile>();
        foreach (var t in allTiles)
        {
            if (t.name == tileName)
                return t;
        }
        return null;
    }

    private NetworkTile FindTileByCoord(int x, int y)
    {
        NetworkTile[] allTiles = FindObjectsOfType<NetworkTile>();
        foreach (var t in allTiles)
        {
            if (t.Coord.x == x && t.Coord.y == y)
                return t;
        }
        return null;
    }

    private void SpawnBuilding(NetworkTile tile, string buildingName, NetworkSide ownerSide, PlayerRef owner, string factionName)
    {
        if (!Runner.IsServer) return;
        
        if(factionName == null)
        {
            Debug.Log($"[NBM] Faction name is null!");
            return;
        }

        MP_Faction faction = factionManager.GetFactionByName(factionName);
        if (faction == null)
        {
            Debug.LogError($"[NBM] Faction not found: {factionName}");
            return;
        }

        GameObject prefab = ResolveBuildingPrefab(faction, buildingName);
        if (prefab == null)
        {
            Debug.LogError($"[NBM] No prefab for {buildingName} in faction {factionName}");
            return;
        }

        Vector3 pos = tile.transform.position + Vector3.up;

        Runner.Spawn(prefab, pos, Quaternion.identity, owner, (runner, obj) =>
        {
            var building = obj.GetComponent<NetworkBuilding>();
            if (building != null)
                building.OwnerSide = ownerSide;
        });

        tile.OccupyTile();
        NetworkCubeGridManager.Instance?.UpdateTileLists();

        Debug.Log($"[NBM] ✓ {buildingName} spawned for {owner} ({factionName})");
        
    }
    
    private GameObject ResolveBuildingPrefab(MP_Faction faction, string buildingName)
    {
        switch (buildingName)
        {
            case "MainBuilding":
            case "MainBuild":
                return faction.mainBuildingPrefab;

            case "UnitBuilding":
            case "UnitBuild":
                return faction.unitBuildingPrefab;

            case "DefenceBuilding":
            case "DefenceTurret":        
                return faction.defenceBuildingPrefab;

            case "ResourceBuilding":
            case "GoldResource":
                return faction.resourceBuildingPrefab;

            default:
                Debug.LogWarning($"[NBM] Unknown building type: {buildingName}");
                return null;
        }
    }

}
