using UnityEngine;

public class MainBuildingSpawner : MonoBehaviour
{
    public static MainBuildingSpawner Instance { get; private set; }

    [Header("Data")]
    public AllFactionsData data;

    [Header("Spawn points")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float yOffset = 1f;

    void Awake() => Instance = this;

    void Start()
    {
        if (data == null)
        {
            Debug.LogError("[Spawner] FactionsData is not assigned.");
            return;
        }

        // Use the faction selected in the menu
        var playerFaction = GameData.SelectedFaction;
        var playerSlots = GetFactionSlots(playerFaction);
        var enemySlots = GetFactionSlots(GetRandomEnemyFaction(playerFaction));

        if (playerSlots == null || enemySlots == null) return;

        SpawnAllBuildings(playerSpawnPoint, playerSlots, Side.Player);
        SpawnAllBuildings(enemySpawnPoint, enemySlots, Side.Enemy);
    }

    GameObject[] GetFactionSlots(FactionName name)
    {
        switch (name)
        {
            case FactionName.Medieval:
                return new[] { data.medievalMainBuilding, data.pastTurretBuilding, data.medievalInfantryBuilding, data.medievalGoldBuilding };
            case FactionName.Present:
                return new[] { data.presentMainBuilding, data.presentTurretBuilding, data.presentInfantryBuilding, data.presentGoldBuilding };
            case FactionName.Futuristic:
                return new[] { data.futureMainBuilding, data.futureTurretBuilding, data.futureInfantryBuilding, data.futureGoldBuilding };
            case FactionName.Galvadore:
                return new[] { data.galvadoreMainBuilding, data.galvadoreTurretBuilding, data.galvadoreInfantryBuilding, data.galvadoreGoldBuilding };
            default: return null;
        }
    }

    FactionName GetRandomEnemyFaction(FactionName player)
    {
        var values = (FactionName[])System.Enum.GetValues(typeof(FactionName));
        FactionName pick;
        do { pick = values[Random.Range(0, values.Length)]; } while (pick == player);
        return pick;
    }

    void SpawnAllBuildings(Transform rootPoint, GameObject[] buildingPrefabs, Side side)
    {
        if (rootPoint == null || buildingPrefabs == null) return;

        SpawnEntry(rootPoint, buildingPrefabs[0], side, "MainBuilding");
    }

    void SpawnEntry(Transform point, GameObject buildingPrefab, Side side, string label)
    {
        if (buildingPrefab == null || buildingPrefab == null) return;

        var pos = point.position + Vector3.up * yOffset;

        Instantiate(buildingPrefab, pos, Quaternion.identity, point);

        Debug.Log($"[Spawner] Spawned {label} for {side}: {buildingPrefab.name}");

        // If this is the main building, mark the tile as occupied
        if (label == "MainBuilding")
        {
            // Find the tile at the spawn point
            if (CubeGridManager.Instance != null)
            {
                Vector2Int coord = CubeGridManager.Instance.WorldToGrid(point.position);
                var tileGO = CubeGridManager.Instance.GetCube(coord);
                if (tileGO != null)
                {
                    var tile = tileGO.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.SetBuildingPlaced();
                        Debug.Log($"[Spawner] Tile at {coord} marked as building placed for {side}");
                    }
                }
            }
        }
    }
}
