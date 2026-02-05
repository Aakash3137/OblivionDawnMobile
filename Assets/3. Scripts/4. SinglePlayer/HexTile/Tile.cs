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

    private SideManager sideManager;
    [ReadOnly]
    public bool isOpen = false; // set true when PlusIcon is activated
    [ReadOnly]
    public bool hasBuilding = false; // NEW flag
    [SerializeField, ReadOnly] private GameObject currentOccupant;
    private Side OldSide;

    public void SetOccupant(GameObject occupant)
    {
        currentOccupant = occupant;
    }
    public void ClearOccupant()
    {
        currentOccupant = null;
    }
    public GameObject GetOccupant()
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
    }

    public void SetOpen(bool open)
    {
        isOpen = open;

        if (myPlusIcon != null)
            myPlusIcon.SetActive(open);

        // if (ownerSide == Side.Enemy)
        //     myPlusIcon.SetActive(false);
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
    }

    // Called when a unit steps onto this tile
    public void Occupy(Side unitSide)
    {
        // Always flip ownership to the entering unit’s side
        // occupant = unit;   // update occupant reference
        SetOwner(unitSide);

    }

    // Called when a unit leaves
    public void Vacate(Side unitSide)
    {
        // Clear occupant only if this unit was tracked
        // if (occupant == unit)
        //     occupant = null;
    }


    // Called when a building is placed
    public void SetBuildingPlaced()
    {
        hasBuilding = true;
    }
}