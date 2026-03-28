using UnityEngine;

public class ScenarioTlesClaim : MonoBehaviour
{
    private Stats stats;
    private Side currentSide => stats.side;
    private CubeGridManager cgmInstance;
    private float updateDelay = 0.1f;

    private void Awake()
    {
        stats = GetComponent<Stats>();
    }

    private void Start()
    {
        cgmInstance = CubeGridManager.Instance;
        _ = UpdateTileOwnerShip();
    }

    private async Awaitable UpdateTileOwnerShip()
    {
        while (gameObject.activeInHierarchy)
        {
            await Awaitable.WaitForSecondsAsync(updateDelay, destroyCancellationToken);
            VerifyOwnerShip();
        }
    }

    #region Tile Ownership
    private void VerifyOwnerShip()
    {
        Vector2Int coord = cgmInstance.WorldToGrid(transform.position);
        var tile = cgmInstance.GetTile(coord);

        if (currentSide == tile.ownerSide)
            return;

        tile.Occupy(currentSide);
    }
    #endregion
}

