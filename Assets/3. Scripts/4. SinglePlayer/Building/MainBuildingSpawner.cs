using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Data")]
    public AllFactionsData data;

    [Header("Spawn points")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float yOffset = 1f;

    private static FactionName EnemyFactionName;

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

        // var enemySlots = GetFactionSlots(GetRandomEnemyFaction(playerFaction));

        // updated enemy main building faction selection through enemy build panel script 
        var enemySlots = GetFactionSlots(EnemyFactionName);

        if (playerSlots == null || enemySlots == null) return;

        SpawnAllBuildings(playerSpawnPoint, playerSlots, Side.Player);
        SpawnAllBuildings(enemySpawnPoint, enemySlots, Side.Enemy);
    }

    // Get faction slots based on faction name
    GameObject[] GetFactionSlots(FactionName name)
    {
        switch (name)
        {
            case FactionName.Medieval:
                return new[] { data.medievalMainBuilding, data.pastTurretBuilding, data.medievalMeleeBuilding, data.medievalGoldBuilding };
            case FactionName.Present:
                return new[] { data.presentMainBuilding, data.presentTurretBuilding, data.presentMeleeBuilding, data.presentGoldBuilding };
            case FactionName.Futuristic:
                return new[] { data.futureMainBuilding, data.futureTurretBuilding, data.futureMeleeBuilding, data.futureGoldBuilding };
            case FactionName.Galvadore:
                return new[] { data.galvadoreMainBuilding, data.galvadoreTurretBuilding, data.galvadoreMeleeBuilding, data.galvadoreGoldBuilding };
            default: return null;
        }
    }


    public static void SetFactionNameThroughEnemyBuildPanel(FactionName enemyFactionName)
    {
        EnemyFactionName = enemyFactionName;
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

        // Any Null references ?
        // Check if something is being called on enable
        // Some references are not yet initialized

        Instantiate(buildingPrefab, pos, Quaternion.identity, point);

        Debug.Log($"[Spawner] Spawned {label} for {side}: {buildingPrefab.name}");

        // If this is the main building, mark the tile as occupied
        if (label == "MainBuilding")
        {
            // Find the tile at the spawn point
            if (CubeGridManager.Instance != null)
            {
                Vector2Int coord = CubeGridManager.Instance.WorldToGrid(point.position);
                var tile = CubeGridManager.Instance.GetCube(coord);
                if (tile != null)
                {
                    tile.SetBuildingPlaced();
                    // Debug.Log($"[Spawner] Tile at {coord} marked as building placed for {side}");
                }
            }
        }
    }
}
