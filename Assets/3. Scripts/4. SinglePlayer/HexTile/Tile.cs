using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Ownership")]
    public Side ownerSide;            // Current owner of the tile

    [Header("Visuals")]
    public Renderer tileRenderer;     // Renderer for the tile mesh

    private SideManager sideManager;
    private UnitSide occupant;        // Track current occupant (optional)

    public bool isOpen = false; // set true when PlusIcon is activated

    public bool hasBuilding = false; // NEW flag

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

        ApplyOwnerMaterial();
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        Transform plusIcon = transform.Find("Cube/Plus_Icon");
        if (plusIcon != null)
            plusIcon.gameObject.SetActive(open);
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
        ownerSide = newOwner;
        ApplyOwnerMaterial();
    }

    // Called when a unit steps onto this tile
    public void Occupy(UnitSide unit)
    {
        // Always flip ownership to the entering unit’s side
        // occupant = unit;   // update occupant reference
        SetOwner(unit.side);
    }

    // Called when a unit leaves
    public void Vacate(UnitSide unit)
    {
        // Clear occupant only if this unit was tracked
        // if (occupant == unit)
        //     occupant = null;
    }


    // Called when a building is placed
    public void SetBuildingPlaced()
    {
        hasBuilding = true;
        isOpen = false;

        // Hide PlusIcon if present
        Transform cubeChild = transform.Find("Cube");
        if (cubeChild != null)
        {
            Transform plusIcon = cubeChild.Find("Plus_Icon");
            if (plusIcon != null) plusIcon.gameObject.SetActive(false);
        }
    }
}
