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

    private ProjectileShooter _projectileShooter;
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

    // Reply target system
    private Stats replyTarget;

    private float targetCheckTimer = 0f;
    private const float targetCheckInterval = 1f;
    private float attackTimer = 0f;

    private void Start()
    {
        _projectileShooter = GetComponent<ProjectileShooter>();
        defenseStats = GetComponent<DefenseBuildingStats>();
        defenseBuildingSO = defenseStats.GetBuildingSO();
        defenseData = defenseStats.GetBuildingData();
        forward = transform.forward;
        
        BattleUnitRegistry.DefenseUnits.Add(defenseStats);

        if (defenseBuilding == null)
            defenseBuilding = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer >= targetCheckInterval)
        {
            targetCheckTimer = 0f;

            if (!IsTargetAlive())
            {
                target = null;
                FindTargetInAttackRange();
            }
        }

        if (target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= defenseData.defenseRangeStats.attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= defenseData.defenseAttackStats.fireRate)
            {
                attackTimer = 0f;
                _projectileShooter.Fire(target);
            }
        }
        else
        {
            attackTimer = 0f;
        }

        LookTarget();
    }

    private void LookTarget()
    {
        if (target == null)
            return;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero && defenseBuilding != null)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            defenseBuilding.transform.rotation = Quaternion.Slerp(defenseBuilding.transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }

    private bool IsTargetAlive()
    {
        return target != null && target.gameObject.activeInHierarchy;
    }

    #region NEW Target System (DTDS)

    public void SetReplyTarget(Stats attacker)
    {
        if (attacker != null && attacker.side != defenseStats.side)
            replyTarget = attacker;
    }

    private void FindTargetInAttackRange()
    {
        Collider[] hits = new Collider[32];
        LayerMask enemyLayerMask = GetEnemyLayerMask();

        int count = Physics.OverlapSphereNonAlloc(transform.position, defenseData.defenseRangeStats.attackRange, hits, enemyLayerMask);

        Stats bestUnit = null;
        Stats bestBuilding = null;
        float bestUnitScore = 0f;
        float bestBuildingScore = 0f;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent(out Stats candidate))
                continue;

            if (candidate.side == defenseStats.side)
                continue;

            float score = candidate.identity.priority;
            score = CalculateScore(candidate, score);

            if (candidate is UnitStats)
            {
                if (score > bestUnitScore)
                {
                    bestUnitScore = score;
                    bestUnit = candidate;
                }
            }
            else if (candidate is BuildingStats || candidate is WallStats || candidate is DefenseBuildingStats)
            {
                if (score > bestBuildingScore)
                {
                    bestBuildingScore = score;
                    bestBuilding = candidate;
                }
            }
        }

        // Assign based on hierarchy: replyTarget > unit > building
        if (replyTarget != null && Vector3.Distance(transform.position, replyTarget.transform.position) <= defenseData.defenseRangeStats.attackRange)
        {
            target = replyTarget;
        }
        else if (bestUnit != null)
        {
            target = bestUnit;
        }
        else
        {
            target = bestBuilding;
        }

        if (target != null)
        {
            if (target is UnitStats)
                primaryTarget = target.gameObject;
            else
                secondaryTarget = target.gameObject;
        }
    }

    private LayerMask GetEnemyLayerMask()
    {
        var attackTargets = defenseBuildingSO.defenseAttackTargets;
        switch (defenseStats.side)
        {
            case Side.Player:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    return LayerMask.GetMask("EnemyAir", "EnemyGround");
                else if (attackTargets.canAttackAir)
                    return LayerMask.GetMask("EnemyAir");
                else
                    return LayerMask.GetMask("EnemyGround");
            case Side.Enemy:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    return LayerMask.GetMask("PlayerAir", "PlayerGround");
                else if (attackTargets.canAttackAir)
                    return LayerMask.GetMask("PlayerAir");
                else
                    return LayerMask.GetMask("PlayerGround");
            default:
                return 0;
        }
    }

    private float CalculateScore(Stats unit, float score)
    {
        float distance = Vector3.Distance(transform.position, unit.transform.position);
        score += distanceWeight / Mathf.Max(distance, 0.1f);

        float healthPercent = unit.currentHealth / unit.basicStats.maxHealth;
        if (healthPercent <= 0.3f) score += lowHealthBonus;

        float angle = Vector3.Angle(forward, unit.transform.position - transform.position);
        if (angle <= defenseBuildingSO.defenseVisionAngles.narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= defenseBuildingSO.defenseVisionAngles.wideViewAngle)
            score += wideAngleBonus;
        else
            score += peripheralAngleBonus;

        return score;
    }

    #endregion
}