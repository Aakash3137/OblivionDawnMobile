// using UnityEngine;

// public class Tile : MonoBehaviour
// {
//     [Header("Ownership")]
//     public Side ownerSide;            // Set manually in Inspector (Player or Enemy)

//     [Header("Visuals")]
//     public Renderer tileRenderer;     // Assign the tile's Renderer in Inspector

//     private SideManager sideManager;

//     void Start()
//     {
//         sideManager = FindAnyObjectByType<SideManager>();

//         // Register this tile in HexGridManager once the manager is initialized
//         if (HexGridManager.Instance != null)
//         {
//             var coord = HexGridManager.Instance.WorldToHex(transform.position);
//             HexGridManager.Instance.RegisterHex(coord, gameObject);
//             Debug.Log("Tile registered at " + coord);
//         }
//         else
//         {
//             Debug.LogError("HexGridManager.Instance is missing! Make sure HexGridManager is in the scene.");
//         }

//         if (tileRenderer == null)
//             tileRenderer = GetComponentInChildren<Renderer>();

//         ApplyOwnerMaterial();
//     }

//     // Apply correct material based on ownerSide
//     public void ApplyOwnerMaterial()
//     {
//         if (sideManager != null && tileRenderer != null)
//             sideManager.SetSide(gameObject, ownerSide);
//     }

//     // Flip ownership (called when combat resolves and winner survives on this tile)
//     public void SetOwner(Side newOwner)
//     {
//         ownerSide = newOwner;
//         ApplyOwnerMaterial();
//     }

//     // Called when a unit steps onto this tile
//     public void Occupy(UnitSide unit)
//     {
//         // You can expand this later if you want to track multiple occupants
//     }

//     // Called when a unit leaves this tile
//     public void Vacate(UnitSide unit)
//     {
//         // Clear occupant tracking if needed
//     }
// }




// using UnityEngine;

// public class Tile : MonoBehaviour
// {
//     [Header("Ownership")]
//     public Side ownerSide;            // Current owner of the tile

//     [Header("Visuals")]
//     public Renderer tileRenderer;     // Renderer for the tile mesh

//     private SideManager sideManager;

//     void Start()
//     {
//         sideManager = FindAnyObjectByType<SideManager>();

//         // Register this tile in HexGridManager
//         if (HexGridManager.Instance != null)
//         {
//             var coord = HexGridManager.Instance.WorldToHex(transform.position);
//             HexGridManager.Instance.RegisterHex(coord, gameObject);
//             Debug.Log("Tile registered at " + coord);
//         }
//         else
//         {
//             Debug.LogError("HexGridManager.Instance is missing!");
//         }

//         if (tileRenderer == null)
//             tileRenderer = GetComponentInChildren<Renderer>();

//         ApplyOwnerMaterial();
//     }

//     // Apply correct material based on ownerSide
//     public void ApplyOwnerMaterial()
//     {
//         if (sideManager != null && tileRenderer != null)
//             sideManager.SetSide(gameObject, ownerSide);
//     }

//     // Flip ownership (called when combat resolves OR unit enters)
//     public void SetOwner(Side newOwner)
//     {
//         ownerSide = newOwner;
//         ApplyOwnerMaterial();
//     }

//     // Called when a unit steps onto this tile
//     // public void Occupy(UnitSide unit)
//     // {
//     //     // If the unit's side is different, flip ownership
//     //     if (unit.side != ownerSide)
//     //     {
//     //         SetOwner(unit.side);
//     //         Debug.Log($"Tile at {transform.position} flipped to {unit.side}");
//     //     }
//     // }


//     public void Occupy(UnitSide unit)
//     {
//         // Always flip ownership to the unit’s side when it enters
//         SetOwner(unit.side);
//         Debug.Log($"Tile at {transform.position} flipped to {unit.side}");
//     }


//     // Called when a unit leaves this tile
//     public void Vacate(UnitSide unit)
//     {
//         // Optional: clear occupant tracking if needed
//     }
// }






// using UnityEngine;

// public class Tile : MonoBehaviour
// {
//     [Header("Ownership")]
//     public Side ownerSide;            // Current owner of the tile

//     [Header("Visuals")]
//     public Renderer tileRenderer;     // Renderer for the tile mesh

//     private SideManager sideManager;
//     private UnitSide occupant;        // Track current occupant

//     void Start()
//     {
//         sideManager = FindAnyObjectByType<SideManager>();

//         if (HexGridManager.Instance != null)
//         {
//             var coord = HexGridManager.Instance.WorldToHex(transform.position);
//             HexGridManager.Instance.RegisterHex(coord, gameObject);
//         }

//         if (tileRenderer == null)
//             tileRenderer = GetComponentInChildren<Renderer>();

//         ApplyOwnerMaterial();
//     }

//     // Apply correct material based on ownerSide
//     public void ApplyOwnerMaterial()
//     {
//         if (sideManager != null && tileRenderer != null)
//             sideManager.SetSide(gameObject, ownerSide);
//     }

//     // Flip ownership
//     public void SetOwner(Side newOwner)
//     {
//         ownerSide = newOwner;
//         ApplyOwnerMaterial();
//     }

//     // Called when a unit steps onto this tile
//     public void Occupy(UnitSide unit)
//     {
//         if (occupant == null)
//         {
//             // Empty tile → unit takes ownership
//             occupant = unit;
//             SetOwner(unit.side);
//         }
//         else
//         {
//             // Tile already occupied
//             if (occupant.side != unit.side)
//             {
//                 // Combat resolution
//                 bool unitWins = ResolveCombat(unit, occupant);

//                 if (unitWins)
//                 {
//                     Destroy(occupant.gameObject); // loser removed
//                     occupant = unit;
//                     SetOwner(unit.side);          // flip to winner
//                 }
//                 else
//                 {
//                     Destroy(unit.gameObject);     // attacker lost
//                     // ownership stays with occupant
//                 }
//             }
//             else
//             {
//                 // Same side → just replace occupant reference
//                 occupant = unit;
//                 SetOwner(unit.side); // still enforce ownership
//             }
//         }
//     }

//     // Called when a unit leaves
//     public void Vacate(UnitSide unit)
//     {
//         if (occupant == unit)
//             occupant = null;
//     }

//     // Stub combat logic — replace with your own
//     private bool ResolveCombat(UnitSide attacker, UnitSide defender)
//     {
//         // Example: random winner
//         return Random.value > 0.5f;
//     }
// }






using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Ownership")]
    public Side ownerSide;            // Current owner of the tile

    [Header("Visuals")]
    public Renderer tileRenderer;     // Renderer for the tile mesh

    private SideManager sideManager;
    private UnitSide occupant;        // Track current occupant

    void Start()
    {
        sideManager = FindAnyObjectByType<SideManager>();

        if (HexGridManager.Instance != null)
        {
            var coord = HexGridManager.Instance.WorldToHex(transform.position);
            HexGridManager.Instance.RegisterHex(coord, gameObject);
        }

        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        ApplyOwnerMaterial();
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
        occupant = unit;
        SetOwner(unit.side);
    }

    // Called when a unit leaves
    public void Vacate(UnitSide unit)
    {
        if (occupant == unit)
            occupant = null;
    }
}
