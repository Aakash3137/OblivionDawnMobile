using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(WeaponManager), typeof(DefenseBuildingStats))]
public class DefenseUnit : MonoBehaviour
{
    [Space(20)]
    [Header("Target priority settings high score = target priority")]
    public float distanceWeight = 20f;
    public float lowHealthBonus = 40f;
    public float narrowAngleBonus = 30f;
    public float wideAngleBonus = 20f;
    public float peripheralAngleBonus = 10f;
    public float checkRadiusOffset = 2.5f;

    private WeaponManager weaponManager;
    private DefenseBuildingStats defenseStats;
    private DefenseBuildingDataSO defenseBuildingSO;
    private DefenseBuildingUpgradeData defenseData;

    [Space(15)]
    public GameObject defenseBuilding;
    private Vector3 forward;

    [field: Header("For Debugging")]
    [field: SerializeField, ReadOnly]
    public Stats target { get; private set; }
    [field: SerializeField, ReadOnly]
    public GameObject primaryTarget { get; private set; }
    [field: SerializeField, ReadOnly]
    public GameObject secondaryTarget { get; private set; }

    private float targetCheckTimer = 0f;
    private const float targetCheckInterval = 1f;
    private float attackTimer = 0f;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        defenseStats = GetComponent<DefenseBuildingStats>();
        defenseBuildingSO = defenseStats.GetBuildingSO();
        defenseData = defenseStats.GetBuildingData();
        forward = transform.forward;
        defenseBuilding = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        // periodic target validation (works for units + buildings)
        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer >= targetCheckInterval)
        {
            targetCheckTimer = 0f;

            if (!isTargetAlive())
            {
                target = null;
                FindTarget();
            }
        }

        // find target
        if (target == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= defenseData.defenseRangeStats.attackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= defenseData.defenseAttackStats.fireRate)
            {
                attackTimer = 0f;
                weaponManager.Fire(target, defenseData.defenseAttackStats.damage, defenseStats.side);
            }
        }

        LookTarget();
    }

    private void LookTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            if (defenseBuilding != null)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                defenseBuilding.transform.rotation = Quaternion.Slerp(defenseBuilding.transform.rotation, lookRot, 10f * Time.deltaTime);
            }
        }
    }

    private bool isTargetAlive()
    {
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        return true;
    }

    #region Find Target
    private void FindTarget()
    {
        Collider[] hits = new Collider[20];

        LayerMask enemyLayerMask = LayerMask.GetMask("PlayerAir", "PlayerGround", "EnemyAir", "EnemyGround");

        var attackTargets = defenseBuildingSO.defenseAttackTargets;

        switch (defenseStats.side)
        {
            case Side.Player:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("EnemyAir", "EnemyGround");
                else if (attackTargets.canAttackAir)
                    enemyLayerMask = LayerMask.GetMask("EnemyAir");
                else if (attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("EnemyGround");
                break;
            case Side.Enemy:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("PlayerAir", "PlayerGround");
                else if (attackTargets.canAttackAir)
                    enemyLayerMask = LayerMask.GetMask("PlayerAir");
                else if (attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("PlayerGround");
                break;
        }

        int count = Physics.OverlapSphereNonAlloc(transform.position, defenseData.defenseRangeStats.detectionRange, hits, enemyLayerMask);

        Stats bestTarget = null;
        float bestScore = 0f;

        for (int i = 0; i < count; i++)
        {
            Stats unit;
            float score;

            if (hits[i].TryGetComponent<UnitStats>(out var unitStat))
            {
                unit = unitStat;
                score = unitStat.identity.priority;
            }
            else if (defenseBuildingSO.canAttackBuildings && hits[i].TryGetComponent<BuildingStats>(out var buildingStats))
            {
                unit = buildingStats;
                score = buildingStats.identity.priority;
            }
            else if (defenseBuildingSO.canAttackWalls && hits[i].TryGetComponent<WallStats>(out var wallStats))
            {
                unit = wallStats;
                score = wallStats.identity.priority;
            }
            else
            {
                continue;
            }

            // Ignore self & same side
            if (unit == this || unit.side == defenseStats.side)
                continue;

            score = CalculateScore(unit, score);

            // Is this a better candidate?
            if (score > bestScore && (target == null || ShouldSwitchTarget(target?.GetComponent<Stats>(), unit)))
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        // Assign target
        if (bestTarget != null && (target == null || bestTarget != target.GetComponent<Stats>()))
        {
            // only assign if switching
            target = bestTarget;

            if (bestTarget is UnitStats)
                primaryTarget = target.gameObject;
            else
                secondaryTarget = target.gameObject;
        }
    }

    private float CalculateScore(Stats unit, float score)
    {
        // Distance score
        float distance = Vector3.Distance(transform.position, unit.transform.position);
        score += distanceWeight / Mathf.Max(distance, 0.1f);

        // Low health bonus
        float healthPercent = unit.currentHealth / unit.basicStats.maxHealth;
        if (healthPercent <= 0.3f) score += lowHealthBonus;

        // View angle
        float angle = Vector3.Angle(forward, unit.transform.position - transform.position);

        if (angle <= defenseBuildingSO.defenseVisionAngles.narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= defenseBuildingSO.defenseVisionAngles.wideViewAngle)
            score += wideAngleBonus;
        else
            score += peripheralAngleBonus;
        return score;
    }

    bool ShouldSwitchTarget(Stats current, Stats candidate)
    {
        // no target, always switch
        if (current == null)
            return true;

        // never switch from unit to building
        if (current is UnitStats && (candidate is BuildingStats || candidate is WallStats))
            return false;

        if (current is BuildingStats || current is WallStats)
        {
            if (candidate == null)
                return false;

            // check if building is low health
            float buildingHealthPercent = current.currentHealth / current.basicStats.maxHealth;
            if (buildingHealthPercent < 0.3f)
                return false; // stick with low-health building

            // if candidate is unit, switch
            if (candidate is UnitStats)
            {
                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                return dist <= defenseData.defenseRangeStats.attackRange + checkRadiusOffset;
            }
        }

        return false; // default: do not switch 
    }
    #endregion    

}
