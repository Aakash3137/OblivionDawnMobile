using UnityEngine;

public class NetworkSideManager : MonoBehaviour
{
    [Header("Materials")]
    public Material playerMaterial;
    public Material enemyMaterial;

    public void SetSide(Renderer rend, NetworkSide side)
    {
        if (rend == null)
            return;

        switch (side)
        {
            case NetworkSide.Player:
                if (playerMaterial != null)
                    rend.sharedMaterial = playerMaterial;
                break;

            case NetworkSide.Enemy:
                if (enemyMaterial != null)
                    rend.sharedMaterial = enemyMaterial;
                break;
        }
    }

    public void SetSide(GameObject tile, NetworkSide side)
    {
        Renderer rend = tile.GetComponentInChildren<Renderer>();
        SetSide(rend, side);
    }
}
