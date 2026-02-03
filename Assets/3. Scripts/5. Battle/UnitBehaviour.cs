/*
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    internal GameObject target;
    
    [Space(20)]
    [Header("Target priority settings high score = target priority")]
    public float distanceWeight = 20f;
    public float lowHealthBonus = 40f;
    public float narrowAngleBonus = 30f;
    public float wideAngleBonus = 20f;
    public float peripheralAngleBonus = 10f;
    public float checkRadiusOffset = 2.5f;
    
    #region Find Target


    private void FindTarget(Stats stats)
    {
        if(stats is OffenseBuildingStats || stats is ResourceBuildingStats || stats is WallStats || stats is DefenseWallStats)
            return;

        if (stats is UnitStats)
        {
            var attackTargets = stats.u
        }
        Collider[] hits = new Collider[20];

        LayerMask enemyLayerMask = LayerMask.GetMask("PlayerAir", "PlayerGround", "EnemyAir", "EnemyGround");

        var attackTargets = stats.;

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
*/
