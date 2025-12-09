using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Building Prefabs")]
    public GameObject playerBuilding; // assign blue building prefab in Inspector
    public GameObject enemyBuilding;  // assign red building prefab in Inspector

    [Header("Side Settings")]
    public Side side; // set in Inspector or at runtime

    void Start()
    {
        ToggleBuilding();
    }

    /// <summary>
    /// Activates the correct building prefab based on side.
    /// </summary>
    public void ToggleBuilding()
    {
        if (side == Side.Player)
        {
            if (playerBuilding != null) playerBuilding.SetActive(true);
            if (enemyBuilding != null) enemyBuilding.SetActive(false);
        }
        else if (side == Side.Enemy)
        {
            if (playerBuilding != null) playerBuilding.SetActive(false);
            if (enemyBuilding != null) enemyBuilding.SetActive(true);
        }
    }

    /// <summary>
    /// Allows changing side at runtime and updates building accordingly.
    /// </summary>
    public void SetSide(Side newSide)
    {
        side = newSide;
        ToggleBuilding();
    }
}




// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Building Variants")]
//     public GameObject[] playerVariants; // assign blue variants in Inspector
//     public GameObject[] enemyVariants;  // assign red variants in Inspector

//     [Header("Side Settings")]
//     public Side side; // set in Inspector or at runtime

//     private GameObject activeBuilding;

//     void Start()
//     {
//         ActivateVariant();
//     }

//     private void ActivateVariant()
//     {
//         // Disable previous active building
//         if (activeBuilding != null) activeBuilding.SetActive(false);

//         // Choose correct array
//         GameObject[] variants = side == Side.Player ? playerVariants : enemyVariants;
//         if (variants == null || variants.Length == 0) return;

//         // Randomly select one
//         int index = Random.Range(0, variants.Length);
//         activeBuilding = variants[index];
//         activeBuilding.SetActive(true);
//     }

//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         ActivateVariant();
//     }
// }
