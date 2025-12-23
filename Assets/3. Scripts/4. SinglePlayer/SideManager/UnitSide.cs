// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Side Settings")]
//     public Side side; // Player or Enemy

//     // [Header("Faction + Building Type")]
//     // public FactionName faction;      // Past, Present, Future, Monster
//     // public string buildingType;      // "MainBuilding", "GoldMine", "UnitBuilding", "TurretBuilding"

//     // This script no longer spawns anything. It only applies materials.

//     public void ApplySideMaterial(AllFactionsData.BuildingSlot slot)
//     {
//         if (slot == null) return;

//         // Pick the correct material based on side
//         var mat = (side == Side.Player) ? slot.playerMaterial : slot.enemyMaterial;
//         if (mat == null) return;

//         // Apply to all renderers in this prefab
//         var renderers = GetComponentsInChildren<Renderer>(true);
//         foreach (var r in renderers)
//         {
//             r.material = mat;
//         }
//     }
// }





using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Side Settings")]
    public Side side; // Player or Enemy

    public void ApplySideMaterial(AllFactionsData.BuildingSlot slot)
    {
        if (slot == null) return;

        // Pick the correct material based on side
        var mat = (side == Side.Player) ? slot.playerMaterial : slot.enemyMaterial;
        if (mat == null) return;

        // Apply only to the first child renderer
        if (transform.childCount > 0)
        {
            var firstChild = transform.GetChild(0);
            var renderer = firstChild.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = mat;
            }
        }
        else
        {
            Debug.LogWarning($"[UnitSide] No child found on {gameObject.name} to apply material.");
        }
    }
}
