using UnityEngine;
using Fusion;
using System.Collections.Generic;

[System.Serializable]
public struct BuildRequestData
{
    public string tileName;
    public string buildingName;
    public int ownerSide;
}
public class NetworkBuildingManager : NetworkBehaviour
{
    public static NetworkBuildingManager Instance;

    [Header("Main Building Settings")]
    public string mainBuildingName = "MainBuilding";
    private bool mainBuildingsSpawned = false;
    private float spawnCheckDelay = 1f;

    private void Awake()
    {
        Instance = this;
        NetworkEventCore.AddListener(EventCode.BuildRequest, OnBuildRequestReceived);
    }

    public override void Spawned()
    {
        if (Runner.IsServer && !mainBuildingsSpawned)
        {
            Invoke(nameof(SpawnMainBuildings), 1f);
        }
    }

    private void SpawnMainBuildings()
    {
        if (mainBuildingsSpawned) return;
        mainBuildingsSpawned = true;

        // Find all NetworkPlayers and sort by SpawnId
        NetworkPlayer[] allPlayers = FindObjectsOfType<NetworkPlayer>();
        
        if (allPlayers.Length < 2)
        {
            Debug.LogWarning($"[NBM] Not enough players to spawn main buildings. Count: {allPlayers.Length}");
            return;
        }

        NetworkPlayer player0 = null;
        NetworkPlayer player1 = null;

        foreach (var player in allPlayers)
        {
            if (player.SpawnId == 0) player0 = player;
            else if (player.SpawnId == 1) player1 = player;
        }

        // Find tiles at (4,4) and (12,12)
        NetworkTile tile1 = FindTileByCoord(4, 4);
        NetworkTile tile2 = FindTileByCoord(12, 12);

        if (tile1 != null && player0 != null)
        {
            // Spawn for player with SpawnId 0
            SpawnBuilding(tile1, mainBuildingName, NetworkSide.Player, player0.Object.InputAuthority);
            Debug.Log($"[NBM] Main building spawned at (4,4) for Player SpawnId 0");
        }

        if (tile2 != null && player1 != null)
        {
            // Spawn for player with SpawnId 1
            SpawnBuilding(tile2, mainBuildingName, NetworkSide.Player, player1.Object.InputAuthority);
            Debug.Log($"[NBM] Main building spawned at (12,12) for Player SpawnId 1");
        }
    }

    // ---------------- CLIENT → HOST ----------------
    public void RequestBuild(NetworkTile tile, string building)
    {
        if (tile == null) return;

        BuildRequestData data = new BuildRequestData
        {
            tileName = tile.name,
            buildingName = building,
            ownerSide = (int)tile.CurrentVisualOwner
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
        
        // Find the client player who sent this request
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
        SpawnBuilding(tile, data.buildingName, ownerSide, requester);
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

    private void SpawnBuilding(NetworkTile tile, string buildingName, NetworkSide ownerSide, PlayerRef owner)
    {
        if (!Runner.IsServer) return;

        BuildingEntry entry = BuildingRegistry.Instance?.GetBuildingEntry(buildingName);
        if (entry?.playerBuildingPrefab == null)
        {
            Debug.LogError($"[NBM] No player prefab found for: {buildingName}");
            return;
        }

        Vector3 pos = tile.transform.position + Vector3.up;
        var spawnedObj = Runner.Spawn(entry.playerBuildingPrefab, pos, Quaternion.identity, owner, (runner, obj) =>
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
            Debug.Log($"[NBM] ✓ {buildingName} spawned for player {owner}");
        }
    }
}
