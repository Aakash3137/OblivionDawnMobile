using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;

public class BuildingPlacementHelper : MonoBehaviour
{
    private Vector2Int currentCoord;
    private CubeGridManager cgmInstance;
    [SerializeField] private List<Tile> neighborTiles = new List<Tile>();
    private Tile currentTile;


    private void Awake()
    {
        cgmInstance = CubeGridManager.Instance;
    }
    private void Start()
    {
        currentCoord = cgmInstance.WorldToGrid(transform.position);
        currentTile = cgmInstance.GetTile(currentCoord);

        GetNeighbors();
        ActivateNeighbors();
        DeactivateSelf();
    }

    private void OnDisable()
    {
        DeactivateNeighbors();
    }

    private void GetNeighbors()
    {
        var neighborCoords = cgmInstance.GetAllNeighbors(currentCoord);

        foreach (var coord in neighborCoords)
        {
            Tile tile = cgmInstance.GetTile(coord);

            if (tile != null)
                neighborTiles.Add(tile);
        }
    }

    public void ActivateNeighbors()
    {
        if (neighborTiles.Count == 0)
            return;

        foreach (var tile in neighborTiles)
        {
            tile.proxyTiles.Add(currentTile);

            if (tile.hasBuilding)
                continue;

            tile.OpenStatusHandler(true);
        }
    }

    private void DeactivateNeighbors()
    {
        if (neighborTiles.Count == 0)
            return;

        // Debug.Log($"<color=green>Cached neighborTile had {neighborTiles.Count} tiles</color>");

        foreach (var tile in neighborTiles)
        {
            tile.proxyTiles.Remove(currentTile);

            if (tile.proxyTiles.Count == 0 || tile.proxyTiles == null)
                tile.OpenStatusHandler(false);

            // if neighbor tile has a building then set current tile to open
            if (tile.hasBuilding)
                ActivateSelf();
        }
    }

    private void DeactivateSelf()
    {
        currentTile.OpenStatusHandler(false);
    }

    private void ActivateSelf()
    {
        currentTile.OpenStatusHandler(true);
    }
}
