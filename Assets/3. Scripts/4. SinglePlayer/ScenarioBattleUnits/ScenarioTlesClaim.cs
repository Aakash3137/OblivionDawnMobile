using UnityEngine;

public class ScenarioTlesClaim : MonoBehaviour
{
    Vector2Int currentCoord;

    private void Update()
    {
        UpdateTileOwnership();
    }

    #region Tile Ownership
    // --- Tile ownership ---
    void UpdateTileOwnership()
    {
        if (CubeGridManager.Instance == null) return;

        Vector2Int coord = CubeGridManager.Instance.WorldToGrid(transform.position);
        if (coord != currentCoord)
        {
            LeaveTile(currentCoord);
            currentCoord = coord;
            EnterTile(currentCoord);
        }
    }

    void EnterTile(Vector2Int coord)
    {
        if (TileManager.Instance != null)
            TileManager.Instance.TryEnterTile(gameObject, coord);

        var tileGO = CubeGridManager.Instance.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
        {
            tile.Occupy(GetComponent<Stats>().side);
        }
    }

    void LeaveTile(Vector2Int coord)
    {
        if (TileManager.Instance != null)
        {
            TileManager.Instance.LeaveTile(coord, gameObject);
            //Debug.Log($"Tile Vacated at {coord}");
        }

        var tileGO = CubeGridManager.Instance?.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
            tile.Vacate(GetComponent<Stats>().side);
    }
    #endregion
}

