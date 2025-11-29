/*using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class NetworkTileOccupancyManager : NetworkBehaviour
{
    public static NetworkTileOccupancyManager Instance;

    // Tracks which tile (hex coord) is occupied by which unit
    [Networked] private NetworkDictionary<Vector2Int, NetworkObject> occupiedTiles { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // --------------------------------------------------------
    // CLIENT REQUESTS MOVEMENT
    // --------------------------------------------------------
    public bool TryRequestEnterTile(NetworkObject unit, Vector2Int coord)
    {
        if (!unit.HasInputAuthority)
            return false; // Only input authority can request

        RPC_RequestEnterTile(unit, coord);
        return true;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestEnterTile(NetworkObject unit, Vector2Int coord, RpcInfo info = default)
    {
        // Server resolves
        if (!occupiedTiles.ContainsKey(coord))
        {
            occupiedTiles[coord] = unit;
        }
        else
        {
            // Tile already occupied, handle combat if needed
            NetworkObject otherUnit = occupiedTiles[coord];
            if (otherUnit != null && otherUnit != unit)
            {
                NetworkCombatManager.Instance.ResolveCombat(unit, otherUnit);
            }
        }
    }

    // Unit leaves a tile (server authoritative)
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_LeaveTile(NetworkObject unit, Vector2Int coord, RpcInfo info = default)
    {
        if (occupiedTiles.TryGetValue(coord, out var current) && current == unit)
            occupiedTiles.Remove(coord);
    }

    // Optional: query
    public NetworkObject GetOccupant(Vector2Int coord)
    {
        occupiedTiles.TryGetValue(coord, out var unit);
        return unit;
    }
}
*/