using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ProjectileShooter))]
public class MainDefenseUnit : MonoBehaviour
{
    [Space(20)]
    [Header("Target priority settings high score = target priority")]
    public float distanceWeight = 20f;
    public float lowHealthBonus = 40f;
    public float narrowAngleBonus = 30f;
    public float wideAngleBonus = 20f;
    public float peripheralAngleBonus = 10f;

    private ProjectileShooter projectileShooter;
    private MainBuildingStats mainStats;
    private MainBuildingDataSO mainBuildingSO;
    private MainBuildingUpgradeData mainData;

    [Space(15)]
    public GameObject[] turretBuilding;
    public List<Transform> firePoint;

    private AttackTargets attackTargets;
    private Vector3 forward;

    [field: SerializeField, ReadOnly, Header("For Debugging")] public Stats[] target { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject[] primaryTarget { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject[] secondaryTarget { get; private set; }

    // Reply target system
    private Stats replyTarget;

    private float[] targetCheckTimer;
    private float[] targetCheckIntervals;
    private float[] attackTimer;

    private void Start()
    {
        projectileShooter = GetComponent<ProjectileShooter>();
        mainStats = GetComponent<MainBuildingStats>();
        mainBuildingSO = mainStats.GetBuildingSO();
        mainData = mainStats.mainBuildingData;
        forward = transform.forward;

        attackTargets = new()
        {
            canAttackAir = true,
            canAttackGround = true
        };

        targetCheckIntervals = new float[turretBuilding.Length];
        targetCheckTimer = new float[turretBuilding.Length];
        attackTimer = new float[turretBuilding.Length];

        target = new Stats[turretBuilding.Length];
        primaryTarget = new GameObject[turretBuilding.Length];
        secondaryTarget = new GameObject[turretBuilding.Length];

        for (int i = 0; i < turretBuilding.Length; i++)
            targetCheckIntervals[i] = Random.Range(0.5f, 1.2f);
    }

    private void Update()
    {
        MultiTargetDetection();
    }

    private void MultiTargetDetection()
    {
        for (int i = 0; i < turretBuilding.Length; i++)
        {
            targetCheckTimer[i] += Time.deltaTime;

            if (targetCheckTimer[i] >= targetCheckIntervals[i])
            {
                targetCheckTimer[i] = 0f;

                if (!IsTargetAlive(i))
                {
                    target[i] = null;
                    FindTargetInAttackRange(i);
                }
            }

            if (target[i] == null)
                return;

            attackTimer[i] += Time.deltaTime;

            if (attackTimer[i] >= mainData.mainAttackStats.fireRate)
            {
                attackTimer[i] = 0f;
                projectileShooter.Fire(target[i], firePoint[i]);
            }

            LookTarget(i);
        }
    }

    private void LookTarget(int index)
    {
        if (target[index] == null)
            return;

        Vector3 dir = (target[index].transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero && turretBuilding != null)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            foreach (var turret in turretBuilding)
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }

    private bool IsTargetAlive(int index)
    {
        return target[index] != null && target[index].gameObject.activeInHierarchy;
    }

    #region NEW Target System (DTDS)

    public void SetReplyTarget(Stats attacker)
    {
        if (attacker != null && attacker.side != mainStats.side)
        {
            if ((attacker.CanFly && attackTargets.canAttackAir) || (!attacker.CanFly && attackTargets.canAttackGround))
            {
                replyTarget = attacker;
            }
        }
    }

    private void FindTargetInAttackRange(int index)
    {
        Collider[] hits = new Collider[32];
        LayerMask enemyLayerMask = GetEnemyLayerMask();

        int count = Physics.OverlapSphereNonAlloc(transform.position, mainData.mainRangeStats.attackRange, hits, enemyLayerMask);

        Stats bestUnit = null;
        Stats bestBuilding = null;
        float bestUnitScore = 0f;
        float bestBuildingScore = 0f;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent(out Stats candidate))
                continue;

            if (candidate.side == mainStats.side)
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
        if (replyTarget != null && Vector3.Distance(transform.position, replyTarget.transform.position) <= mainData.mainRangeStats.attackRange)
        {
            target[index] = replyTarget;
        }
        else if (bestUnit != null)
        {
            target[index] = bestUnit;
        }
        else
        {
            target[index] = bestBuilding;
        }

        if (target[index] != null)
        {
            if (target[index] is UnitStats)
                primaryTarget[index] = target[index].gameObject;
            else
                secondaryTarget[index] = target[index].gameObject;
        }
    }

    private LayerMask GetEnemyLayerMask()
    {
        switch (mainStats.side)
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
        if (angle <= mainBuildingSO.mainVisionAngles.narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= mainBuildingSO.mainVisionAngles.wideViewAngle)
            score += wideAngleBonus;
        else
            score += peripheralAngleBonus;

        return score;
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        var AttackRange = mainData.mainRangeStats.attackRange;
        var DetectionRange = mainData.mainRangeStats.detectionRange;
        var MinAttackRange = mainData.mainRangeStats.minAttackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        if (MinAttackRange > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, MinAttackRange);
        }
    }

    #endregion
}