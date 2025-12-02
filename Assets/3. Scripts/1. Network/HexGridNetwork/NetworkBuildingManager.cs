using UnityEngine;
using Fusion;

[System.Serializable]
public struct BuildRequestData
{
    public string tileName;      // Use the tile's unique name or coordinate
    public string buildingName;
}

public class NetworkBuildingManager : NetworkBehaviour
{
    public static NetworkBuildingManager Instance;

    public GameObject housePrefab;

    private void Awake()
    {
        Instance = this;

        // Listen to build events
        NetworkEventCore.AddListener(EventCode.BuildRequest, OnBuildRequestReceived);
        NetworkEventCore.AddListener(EventCode.BuildSync, OnBuildSyncReceived);
    }

    // ---------------- CLIENT → HOST ----------------
    public void RequestBuild(NetworkTile tile, string building)
    {
        if (tile == null) return;

        BuildRequestData data = new BuildRequestData
        {
            tileName = tile.name,
            buildingName = building
        };

        if (Runner.IsServer)
        {
            Debug.Log($"[NBM] Host clicked tile {tile.name}, processing locally.");
            ProcessBuildRequest(data, isHost: true);
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
        ProcessBuildRequest(data, isHost: true);
    }

    // ---------------- CLIENT RECEIVES ----------------
    private void OnBuildSyncReceived(string json)
    {
        if (Runner.IsServer) return;

        BuildRequestData data = JsonUtility.FromJson<BuildRequestData>(json);
        Debug.Log($"[NBM] Client received build sync for tile {data.tileName}, building: {data.buildingName}");
       
        //client cannot spawn anything
        //ProcessBuildRequest(data, isHost: false);
    }

    // ---------------- SPAWN LOGIC ----------------
    private void ProcessBuildRequest(BuildRequestData data, bool isHost)
    {
        // Find the tile by name (you could also use HexCoord.ToString())
        NetworkTile tile = FindTileByName(data.tileName);
        if (tile == null)
        {
            Debug.LogError($"[NBM] Tile {data.tileName} not found!");
            return;
        }

        if (isHost)
        {
            Debug.Log($"[NBM] Host spawning building {data.buildingName} on tile {tile.name}");
            SpawnBuilding(tile);

            // Notify clients
            NetworkEventCore.RaiseEvent(EventCode.BuildSync, data, NetworkEventTargets.ClientsOnly);
        }
        /*else
        {
            Debug.Log($"[NBM] Client spawning building {data.buildingName} locally on tile {tile.name}");
            SpawnBuilding(tile);
        }*/
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

    private void SpawnBuilding(NetworkTile tile)
    {
        if (!Runner.IsServer)
        {
            Debug.LogWarning("[NBM] Client attempted to spawn — blocked.");
            return;
        }

        Vector3 pos = tile.transform.position + Vector3.up;
        Runner.Spawn(housePrefab, pos, Quaternion.identity);

        Debug.Log($"[NBM] Spawned building on tile {tile.name} (server)");
    }
}
