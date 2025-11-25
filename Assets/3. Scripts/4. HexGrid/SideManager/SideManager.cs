using UnityEngine;

public class SideManager : MonoBehaviour
{
    [Header("Materials")]
    public Material playerMaterial;
    public Material enemyMaterial;

    // Assign side to a prefab
    public void SetSide(GameObject unitOrBuilding, Side side)
    {
        Renderer renderer = unitOrBuilding.GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        switch (side)
        {
            case Side.Player:
                renderer.material = playerMaterial;
                break;
            case Side.Enemy:
                renderer.material = enemyMaterial;
                break;
        }
    }
}
