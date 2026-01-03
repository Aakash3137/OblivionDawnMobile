using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitSpawner : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private List<Button> playerSpawnButtons;
    [SerializeField] private List<Button> enemySpawnButtons;

    [Header("Unit Prefabs")]
    [SerializeField] private List<GameObject> playerUnits;
    [SerializeField] private List<GameObject> enemyUnits;

    [Header("Spawn Points (0 = Center)")]
    [SerializeField] private List<Transform> playerSpawnPoints;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [Header("Unit Pools")]
    [SerializeField] private Transform playerUnitPool;
    [SerializeField] private Transform enemyUnitPool;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnCooldown = 0.1f;
    [SerializeField] private float fastClickThreshold = 1f;

    private HashSet<int> usedPlayerSpawnPoints = new HashSet<int>();
    private HashSet<int> usedEnemySpawnPoints = new HashSet<int>();
    
    // --- Player tracking ---
    private float lastPlayerClickTime = -10f;
    private float lastPlayerSpawnTime;
    private int playerFastSpawnIndex = 1;

    // --- Enemy tracking ---
    private float lastEnemyClickTime = -10f;
    private float lastEnemySpawnTime;
    private int enemyFastSpawnIndex = 1;

    private void Start()
    {
        RegisterButtons(playerSpawnButtons, SpawnPlayerUnit);
        RegisterButtons(enemySpawnButtons, SpawnEnemyUnit);
    }

    #region Button Registration

    private void RegisterButtons(List<Button> buttons, System.Action<Button> callback)
    {
        foreach (Button button in buttons)
        {
            Button btn = button; // closure fix
            btn.onClick.AddListener(() => callback(btn));
        }
    }

    #endregion

    #region Spawn Logic

    private void SpawnPlayerUnit(Button button)
    {
        if (!CanSpawn(ref lastPlayerSpawnTime))
            return;

        GameObject prefab = GetUnitPrefab(playerUnits, button);
        
        Transform spawnPoint = GetPlayerSpawnPoint();
        if (spawnPoint == null)
            return; // STOP spawning

        Instantiate(prefab, spawnPoint.position, Quaternion.identity, playerUnitPool);

        //instantiate the unit and set the target
    }

    private void SpawnEnemyUnit(Button button)
    {
        if (!CanSpawn(ref lastEnemySpawnTime))
            return;

        GameObject prefab = GetUnitPrefab(enemyUnits, button);
        if (prefab == null) return;

        Transform spawnPoint = GetEnemySpawnPoint();
        
        if (spawnPoint == null)
            return; // STOP spawning

        Instantiate(prefab, spawnPoint.position, Quaternion.identity, enemyUnitPool);
    }

    #endregion

    #region Spawn Point Selection (FAST vs NORMAL)
    private Transform GetPlayerSpawnPoint()
    {
        if (!BattleUnit.AnyPlayerAlive())
        {
            usedPlayerSpawnPoints.Clear();
        }
        
        bool anyTarget = BattleUnit.AnyPlayerHasTarget();

        // NO TARGET → limited spawning
        if (!anyTarget)
        {
            for (int i = 0; i < playerSpawnPoints.Count; i++)
            {
                if (!usedPlayerSpawnPoints.Contains(i))
                {
                    usedPlayerSpawnPoints.Add(i);
                    return playerSpawnPoints[i];
                }
            }

            // All spawn points used → STOP spawning
            return null;
        }

        // TARGET EXISTS → reset restrictions
        usedPlayerSpawnPoints.Clear();

        // normal fast-click logic (cycle freely)
        Transform point = playerSpawnPoints[playerFastSpawnIndex];
        playerFastSpawnIndex = (playerFastSpawnIndex + 1) % playerSpawnPoints.Count;
        return point;
    }

    private Transform GetEnemySpawnPoint()
    {
        if (!BattleUnit.AnyEnemyAlive())
        {
            usedEnemySpawnPoints.Clear();
        }

        
        bool anyTarget = BattleUnit.AnyEnemyHasTarget();

        if (!anyTarget)
        {
            for (int i = 0; i < enemySpawnPoints.Count; i++)
            {
                if (!usedEnemySpawnPoints.Contains(i))
                {
                    usedEnemySpawnPoints.Add(i);
                    return enemySpawnPoints[i];
                }
            }

            return null;
        }

        usedEnemySpawnPoints.Clear();

        Transform point = enemySpawnPoints[enemyFastSpawnIndex];
        enemyFastSpawnIndex = (enemyFastSpawnIndex + 1) % enemySpawnPoints.Count;
        return point;
    }

    #endregion

    #region Helpers

    private bool CanSpawn(ref float lastSpawnTime)
    {
        if (Time.time - lastSpawnTime < spawnCooldown)
            return false;

        lastSpawnTime = Time.time;
        return true;
    }

    private GameObject GetUnitPrefab(List<GameObject> units, Button button)
    {
        UnitButton unitButton = button.GetComponent<UnitButton>();
        if (unitButton == null) return null;

        foreach (GameObject unit in units)
        {
            BattleUnit battleUnit = unit.GetComponent<BattleUnit>();
            if (battleUnit != null &&
                battleUnit.battleUnitEnum.ToString() == unitButton.buttonname)
            {
                return unit;
            }
        }

        Debug.LogWarning($"No unit prefab found for button: {unitButton.buttonname}");
        return null;
    }

    #endregion
}
