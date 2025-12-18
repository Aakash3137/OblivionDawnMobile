using System.Collections.Generic;
using UnityEngine;

public class NetworkBuildingPlacementHelper : MonoBehaviour
{
    private List<Transform> activatedIcons = new List<Transform>();
    private List<NetworkTile> openedTiles = new List<NetworkTile>();

    [Header("Neighbor Settings")]
    public bool useDiagonals = false;   // toggle in inspector: 4-way vs 8-way neighbors

    private Vector2Int currentCoord;    // track where building is snapped

    void Start()
    {
        // Only show Plus icons if this building belongs to local player
        var building = GetComponent<NetworkBuilding>();
        if (building != null && building.Object != null && building.Object.HasInputAuthority)
        {
            SnapAndActivate();
        }
    }

    void Update()
    {
        // Only update if this is local player's building
        var building = GetComponent<NetworkBuilding>();
        if (building == null || building.Object == null || !building.Object.HasInputAuthority)
            return;

        var gm = NetworkCubeGridManager.Instance;
        if (gm == null) return;

        Vector2Int newCoord = gm.WorldToGrid(transform.position);
        if (newCoord != currentCoord)
        {
            DeactivateNeighbors();
            SnapAndActivate();
        }
    }

    void OnDestroy()
    {
        DeactivateNeighbors();
    }

    private void SnapAndActivate()
    {
        var gm = NetworkCubeGridManager.Instance;
        if (gm == null) return;

        currentCoord = GetClosestCoord(transform.position);

        Vector3 center = gm.GridToWorld(currentCoord);
        transform.position = new Vector3(center.x, transform.position.y, center.z);

        List<Vector2Int> neighbors = useDiagonals
            ? gm.GetAllNeighbors(currentCoord)
            : gm.GetCardinalNeighbors(currentCoord);

        foreach (var coord in neighbors)
        {
            GameObject tileObj = gm.GetCube(coord);
            if (tileObj == null) continue;

            NetworkTile tileScript = tileObj.GetComponent<NetworkTile>();
            if (tileScript == null) continue;

            if (tileScript.CurrentVisualOwner == NetworkSide.Enemy)
                continue;

            // Skip tiles that already have a building
            if (tileScript.IsOccupied)
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
                    NetworkCubeGridManager.Instance.SelectableTiles.Add(tileScript);
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
            if (tile != null && tile.IsOccupied) // keep closed unless building exists
                tile.isOpen = false;
        }
        openedTiles.Clear();
    }

    private Vector2Int GetClosestCoord(Vector3 worldPos)
    {
        var gm = NetworkCubeGridManager.Instance;
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
