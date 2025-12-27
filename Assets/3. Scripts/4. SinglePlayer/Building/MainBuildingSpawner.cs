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

    AllFactionsData.BuildingSlot[] GetFactionSlots(FactionName name)
    {
        switch (name)
        {
            case FactionName.Past:
                return new[] { data.pastMainBuilding, data.pastTurretBuilding, data.pastUnitBuilding, data.pastGoldMine };
            case FactionName.Present:
                return new[] { data.presentMainBuilding, data.presentTurretBuilding, data.presentUnitBuilding, data.presentGoldMine };
            case FactionName.Future:
                return new[] { data.futureMainBuilding, data.futureTurretBuilding, data.futureUnitBuilding, data.futureGoldMine };
            case FactionName.Monster:
                return new[] { data.monsterMainBuilding, data.monsterTurretBuilding, data.monsterUnitBuilding, data.monsterGoldMine };
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

    void SpawnAllBuildings(Transform rootPoint, AllFactionsData.BuildingSlot[] slots, Side side)
    {
        if (rootPoint == null || slots == null) return;

        SpawnEntry(rootPoint, slots[0], side, "MainBuilding");
    }

 void SpawnEntry(Transform point, AllFactionsData.BuildingSlot slot, Side side, string label)
    {
        if (slot == null || slot.prefab == null) return;

        var pos = point.position + Vector3.up * yOffset;
        var go = Instantiate(slot.prefab, pos, Quaternion.identity, point);

        var unitSide = go.GetComponent<SideScenario>();
        if (unitSide != null)
        {
            unitSide.side = side;
            unitSide.ApplySideMaterial(slot);
        }

        Debug.Log($"[Spawner] Spawned {label} for {side}: {slot.prefab.name}");

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
