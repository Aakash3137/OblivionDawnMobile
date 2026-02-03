using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Ownership")]
    public Side ownerSide;            // Current owner of the tile

    [Header("Visuals")]
    public Renderer tileRenderer;     // Renderer for the tile mesh

    private SideManager sideManager;
    public bool isOpen = false; // set true when PlusIcon is activated

    public bool hasBuilding = false; // NEW flag
    private GameObject currentOccupant;
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

    void Start()
    {
        sideManager = FindAnyObjectByType<SideManager>();

        if (CubeGridManager.Instance != null)
        {
            var coord = CubeGridManager.Instance.WorldToGrid(transform.position);
            CubeGridManager.Instance.RegisterCube(coord, gameObject);
        }

        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        OldSide = ownerSide;
        ApplyOwnerMaterial();
    }

    public void SetOpen(bool open)
    {
        isOpen = open;

        Transform plusIcon = transform.Find("Cube/Plus_Icon");
        if (plusIcon != null)
        {
            // Only show PlusIcon for Player side
            if (ownerSide == Side.Player)
            {
                plusIcon.gameObject.SetActive(open);
            }
            else
            {
                // Enemy side never shows PlusIcon
                plusIcon.gameObject.SetActive(false);
            }
        }

        //// Debug.Log($"Tile at {transform.position} open={isOpen}, side={ownerSide}");
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
        isOpen = false;
        hasBuilding = true;

        // Hide PlusIcon if present
        Transform cubeChild = transform.Find("Cube");
        if (cubeChild != null && ownerSide != Side.Enemy)
        {
            Transform plusIcon = cubeChild.Find("Plus_Icon");
            if (plusIcon != null) plusIcon.gameObject.SetActive(false);
        }
    }
}