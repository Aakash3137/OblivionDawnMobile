using UnityEngine;

public class SideManager : MonoBehaviour
{
    [Header("Materials")]
    public Material playerMaterial;
    public Material enemyMaterial;
    public Material neutralAllyMaterial;
    public Material neutralEnemyMaterial;

    // Assign side to a prefab
    public void SetSide(GameObject unitOrBuilding, Side side)
    {
        Renderer renderer = unitOrBuilding.GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        switch (side)
        {
            case Side.Player:
                if (playerMaterial != null)
                    renderer.material = playerMaterial;
                break;
            case Side.Enemy:
                if (enemyMaterial != null)
                    renderer.material = enemyMaterial;
                break;
            case Side.NeutralAlly:
                if (neutralAllyMaterial != null)
                    renderer.material = neutralAllyMaterial;
                break;
            case Side.NeutralEnemy:
                if (neutralEnemyMaterial != null)
                    renderer.material = neutralEnemyMaterial;
                break;
        }
    }
}
