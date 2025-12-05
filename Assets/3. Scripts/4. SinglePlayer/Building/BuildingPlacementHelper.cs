using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacementHelper : MonoBehaviour
{
    private List<Transform> activatedIcons = new List<Transform>();
    private List<Tile> openedTiles = new List<Tile>();

    [Header("Neighbor Settings")]
    public bool useDiagonals = false;   // toggle in inspector: 4-way vs 8-way neighbors

    private Vector2Int currentCoord;    // track where building is snapped

    void Start()
    {
        SnapAndActivate();
    }

    void Update()
    {
        // Check if building has moved to a new tile
        var gm = CubeGridManager.Instance;
        if (gm == null) return;

        Vector2Int newCoord = gm.WorldToGrid(transform.position);
        if (newCoord != currentCoord)
        {
            // Building moved → deactivate old neighbors, activate new ones
            DeactivateNeighbors();
            SnapAndActivate();
        }
    }

    void OnDestroy()
    {
        DeactivateNeighbors();
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    private void SnapAndActivate()
    {
        var gm = CubeGridManager.Instance;
        if (gm == null) return;

        // Find closest cube tile
        currentCoord = GetClosestCoord(transform.position);

        // Snap building to tile center (keep Y)
        Vector3 center = gm.GridToWorld(currentCoord);
        transform.position = new Vector3(center.x, transform.position.y, center.z);

        // Get neighbors
        List<Vector2Int> neighbors = useDiagonals
            ? gm.GetAllNeighbors(currentCoord)
            : gm.GetCardinalNeighbors(currentCoord);

        foreach (var coord in neighbors)
        {
            GameObject tileObj = gm.GetCube(coord);
            if (tileObj == null) continue;

            Tile tileScript = tileObj.GetComponent<Tile>();
            if (tileScript == null) continue;

            if (tileScript.ownerSide == Side.Enemy)
                continue;

            // Show PlusIcon
            Transform cubeChild = tileObj.transform.Find("Cube");
            if (cubeChild != null)
            {
                Transform plusIcon = cubeChild.Find("Plus_Icon");
                if (plusIcon != null)
                {
                    plusIcon.gameObject.SetActive(true);
                    activatedIcons.Add(plusIcon);
                }
            }

            tileScript.isOpen = true;
            openedTiles.Add(tileScript);
        }
    }

    private void DeactivateNeighbors()
    {
        foreach (var icon in activatedIcons)
        {
            if (icon != null) icon.gameObject.SetActive(false);
        }
        activatedIcons.Clear();

        foreach (var tile in openedTiles)
        {
            if (tile != null) tile.isOpen = false;
        }
        openedTiles.Clear();
    }

    private Vector2Int GetClosestCoord(Vector3 worldPos)
    {
        var gm = CubeGridManager.Instance;
        Vector2Int best = Vector2Int.zero;
        float bestDist = float.MaxValue;

        foreach (var kv in gm.cubeTiles)
        {
            Vector3 tileCenter = gm.GridToWorld(kv.Key);
            float d = Vector3.SqrMagnitude(
                new Vector3(worldPos.x, 0, worldPos.z) -
                new Vector3(tileCenter.x, 0, tileCenter.z)
            );
            if (d < bestDist)
            {
                bestDist = d;
                best = kv.Key;
            }
        }
        return best;
    }
}
