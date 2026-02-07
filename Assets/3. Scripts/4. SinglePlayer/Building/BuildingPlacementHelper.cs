using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacementHelper : MonoBehaviour
{
    private Vector2Int currentCoord;
    private CubeGridManager cgmInstance;
    [SerializeField] private bool allNeighbors = true;
    [SerializeField] private List<Tile> neighborTiles = new List<Tile>();
    private Tile currentTile;


    private void Awake()
    {
        cgmInstance = CubeGridManager.Instance;
    }

    private void GetNeighbors(Side side = Side.Player)
    {
        neighborTiles.Clear();

        currentTile = cgmInstance.GetCube(currentCoord);
        currentTile.isRegistered = true;

        List<Vector2Int> neighborCoords;

        if (currentTile.ownerSide == Side.Enemy)
            return;

        if (allNeighbors)
            neighborCoords = cgmInstance.GetAllNeighbors(currentCoord);
        else
            neighborCoords = cgmInstance.GetCardinalNeighbors(currentCoord);

        foreach (var coord in neighborCoords)
        {
            Tile tile = cgmInstance.GetCube(coord);

            if (tile == null || tile.ownerSide != side)
                continue;

            if (tile.isRegistered) //|| tile.currentOccupant.buildingType == ScenarioBuildingType.MainBuilding)
                continue;

            neighborTiles.Add(tile);
            tile.isRegistered = true;
        }
    }

    public void ActivateNeighbors()
    {
        GetNeighbors();

        if (neighborTiles.Count == 0)
            return;

        foreach (var tile in neighborTiles)
        {
            if (tile.hasBuilding)
                continue;

            tile.SetOpen(true);
        }
    }

    private void DeactivateNeighbors()
    {
        if (neighborTiles.Count == 0)
            return;
        // List<Tile> builtTiles = new List<Tile>();

        // Debug.Log($"<color=green>Cached neighborTile had {neighborTiles.Count} tiles</color>");

        foreach (var tile in neighborTiles)
        {
            // if (tile == null)
            //     Debug.Log("<color=green>Cached neighborTile is null</color>");
            // else
            //     Debug.Log($"<color=green>{tile.name}</color>");

            tile.SetOpen(false);

            if (tile.hasBuilding)
                ActivateSelf();

            // if (tile.hasBuilding)
            //     builtTiles.Add(tile);
        }

        // Debug.Log($"<color=green>Found {builtTiles.Count} built tiles</color>");

        // foreach (var item in builtTiles)
        // {
        //     Debug.Log($"<color=green>Activating neighbors for {item.name}</color>");
        //     item.GetOccupant()?.GetComponent<BuildingPlacementHelper>()?.ActivateNeighbors();
        // }
    }
    private void DeactivateSelf()
    {
        currentTile.SetOpen(false);
    }
    private void ActivateSelf()
    {
        currentTile.SetOpen(true);
    }

    private void Start()
    {
        currentCoord = cgmInstance.WorldToGrid(transform.position);
        ActivateNeighbors();
        DeactivateSelf();
    }

    private void OnDisable()
    {
        DeactivateNeighbors();
    }
}
