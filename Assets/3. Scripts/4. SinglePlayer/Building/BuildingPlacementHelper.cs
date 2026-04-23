using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;

public class BuildingPlacementHelper : MonoBehaviour
{
    private Vector2Int currentCoord;
    private CubeGridManager cgmInstance;
    [SerializeField] private List<Tile> neighborTiles = new List<Tile>();
    internal static Tile currentTile;
    [SerializeField] internal Transform GlowEffectPlace, RepairEffectPlace;


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
    public static Tile GetTile()
    {
        return currentTile;
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

    #region Click Event & Repair
    RepairButtonHandler RepairObj = null;
    void OnMouseDown()
    {
        if (GameStateManager.Instance.IsGameOver) 
            return;

        Stats _CurrentStats = gameObject.GetComponent<Stats>();
        Debug.Log("Clicked on " + gameObject.name);
        RepairObj = RepairManager.Instance.OnClickRepairBtnOpen(RepairEffectPlace, RepairEffectPlace, _CurrentStats, false);

        if(RepairObj == null)
            return;
        
        RepairObj.CurrentWall = GetWallParentFromBuilding(gameObject);
        if(_CurrentStats.currentHealth > _CurrentStats.basicStats.maxHealth/2)
            RepairObj.Repairbtn.interactable = false;
           else
             RepairObj.Repairbtn.interactable = true;

        if(_CurrentStats is BuildingStats buildingStats)
        {
            if(buildingStats.buildingType == ScenarioBuildingType.MainBuilding)
            {
                RepairObj.IsMain = true;
            }
        }
        _CurrentStats.RepairObj = RepairObj;
        // StartCoroutine(CoolDownTimerStart());
    }

    public static WallParent GetWallParentFromBuilding(GameObject building)
    {
        WallParent wallParent = building.GetComponentInChildren<WallParent>();
        if (wallParent == null)
        {
            Debug.Log($"<color=yellow>No WallParent found in children of {building.name}</color>");
            return null;
        }
        return wallParent;
    }
#endregion
}
