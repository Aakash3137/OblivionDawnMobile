using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Ownership"), ReadOnly]
    public Side ownerSide;            // Current owner of the tile

    [Header("Visuals"), ReadOnly]
    public Renderer tileRenderer;     // Renderer for the tile mesh
    [field: SerializeField, ReadOnly]
    public GameObject myPlusIcon { get; private set; }

    [SerializeField] GameObject borderNorth;
    [SerializeField] GameObject borderSouth;
    [SerializeField] GameObject borderEast;
    [SerializeField] GameObject borderWest;

    private SideManager sideManager;
    [ReadOnly] public bool isOpen = false; // set true when PlusIcon is activated
    [field: SerializeField, ReadOnly] public bool hasBuilding { get; private set; }
    // [ReadOnly] public bool isRegistered = false;


    [field: SerializeField, ReadOnly]
    public BuildingStats currentOccupant { get; private set; }
    private Side OldSide;

    [ReadOnly] public List<Tile> proxyTiles = new List<Tile>();

    public void SetOccupant(BuildingStats occupant)
    {
        currentOccupant = occupant;
        occupant.SetBuildingSide(ownerSide);
        hasBuilding = true;
    }
    public void ClearOccupant()
    {
        currentOccupant = null;
        hasBuilding = false;
    }
    public BuildingStats GetOccupant()
    {
        return currentOccupant;
    }
    private void OnValidate()
    {
        if (myPlusIcon == null)
            myPlusIcon = transform.Find("Cube/Plus_Icon")?.gameObject;
    }
    

    void Start()
    {
        sideManager = FindAnyObjectByType<SideManager>();

        if (CubeGridManager.Instance != null)
        {
            var coord = CubeGridManager.Instance.WorldToGrid(transform.position);
            CubeGridManager.Instance.RegisterCube(coord, this);
        }

        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        if (myPlusIcon == null)
        {
            Debug.Log("<color=red>Plus_Icon not found</color>");
            myPlusIcon = transform.Find("Cube/Plus_Icon")?.gameObject;
        }

        OldSide = ownerSide;
        ApplyOwnerMaterial();

        UpdateBorders();
        UpdateNeighborBorders();
    }


public void UpdateBorders()
{
    var grid = CubeGridManager.Instance;
    var coord = grid.WorldToGrid(transform.position);

    Tile north = grid.GetCube(coord + new Vector2Int(0,1));
    Tile south = grid.GetCube(coord + new Vector2Int(0,-1));
    Tile east = grid.GetCube(coord + new Vector2Int(1,0));
    Tile west = grid.GetCube(coord + new Vector2Int(-1,0));

    if(ownerSide != Side.Enemy)
    {
        borderNorth.SetActive(false);
        borderSouth.SetActive(false);
        borderEast.SetActive(false);
        borderWest.SetActive(false);
        return;
    }

    borderNorth.SetActive(north == null || north.ownerSide != Side.Enemy);
    borderSouth.SetActive(south == null || south.ownerSide != Side.Enemy);
    borderEast.SetActive(east == null || east.ownerSide != Side.Enemy);
    borderWest.SetActive(west == null || west.ownerSide != Side.Enemy);
}

    public void SetOpen(bool open)
    {
        isOpen = open;

        if (myPlusIcon == null)
            return;

        if (ownerSide == Side.Enemy)
        {
            myPlusIcon.SetActive(false);
            return;
        }

        myPlusIcon.SetActive(open);
    }

    // Apply correct material based on ownerSide
    public void ApplyOwnerMaterial()
    {
        if (sideManager != null && tileRenderer != null)
            sideManager.SetSide(gameObject, ownerSide);
    }

    // Flip ownership
    public void SetOwner(Side newOwner)
    {
        // GameDebug.Log($"Occupy called on tile at {ownerSide}");
        OldSide = ownerSide;
        ownerSide = newOwner;
        // GameDebug.Log($"Occupy called on tile at {OldSide}, new owner: {ownerSide} ");
        TileCounterUI.Instance.UpdateTileOwnerCount(OldSide, ownerSide);
        ApplyOwnerMaterial();

        UpdateBorders();
        UpdateNeighborBorders();

    }


    void UpdateNeighborBorders()
    {
        var grid = CubeGridManager.Instance;
        var coord = grid.WorldToGrid(transform.position);

        Tile[] neighbors =
        {
            grid.GetCube(coord + new Vector2Int(0,1)),
            grid.GetCube(coord + new Vector2Int(0,-1)),
            grid.GetCube(coord + new Vector2Int(1,0)),
            grid.GetCube(coord + new Vector2Int(-1,0))
        };

        foreach(var n in neighbors)
        {
            if(n != null)
                n.UpdateBorders();
        }
    }
    // Called when a unit steps onto this tile
    public void Occupy(Side unitSide)
    {
        // Always flip ownership to the entering unit’s side
        // occupant = unit;   // update occupant reference
        if (ownerSide == Side.NeutralAlly || ownerSide == Side.NeutralEnemy)
            return;

        SetOwner(unitSide);

        SetOpen(isOpen);
    }

    // Called when a unit leaves
    public void Vacate(Side unitSide)
    {
        // Clear occupant only if this unit was tracked
        // if (occupant == unit)
        //     occupant = null;
    }
}