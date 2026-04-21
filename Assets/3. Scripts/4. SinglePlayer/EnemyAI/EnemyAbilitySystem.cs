using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactionAbilitySet
{
    public FactionName faction;
    public List<SpecialAbilityData> abilities;
}

public class EnemyAbilitySystem : MonoBehaviour
{
    public List<FactionAbilitySet> factionAbilities;

    public float currentCharge = 0f;
    public float maxCharge = 100f;
    public float chargePerKill = 5f;

    [Header("Ability Choice Settings")]
    [Range(0f, 100f)]
    public float specialAbilityChance = 01f;

    public LayerMask playerLayer;
    public float scanRadius = 50f;
    [SerializeField] private Transform scanCenter;
    
    private bool isCasting = false;
    private int abilityIndex = 0;
    private bool abilityQueued = false;
    [SerializeField]private List<AbilitySO> enemyUnitAbilities = new List<AbilitySO>();

    private void OnEnable()
    {
        KillCounterManager.OnUnitKilled += OnUnitKilled;
    }

    private void OnDisable()
    {
        KillCounterManager.OnUnitKilled -= OnUnitKilled;
    }

    private void OnUnitKilled(UnitProduceStatsSO stats, Side deadSide)
    {
        if (deadSide != Side.Player) return;

        currentCharge += stats.populationCost * chargePerKill;
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);

        if (!isCasting && currentCharge >= maxCharge)
        {
            abilityQueued = true;
            StartCoroutine(CastAbilityRoutine());
        }
    }

    IEnumerator CastAbilityRoutine()
    {
        isCasting = true;

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        float roll = Random.Range(0f, 100f);

        bool trySpecial = currentCharge >= maxCharge && roll < specialAbilityChance;

        if (trySpecial)
        {
            SpecialAbilityData specialAbility = PickSpecialAbility();

            if (specialAbility != null)
            {
                Vector3 bestTarget = FindBestTargetPosition(specialAbility.damageArea);
                ExecuteSpecialAbility(specialAbility, bestTarget);

                currentCharge = 0f;
            }
        }
        else
        {
            AbilitySO unitAbility = PickUnitAbility();

            if (unitAbility != null)
            {
                ExecuteUnitAbility(unitAbility);

                currentCharge -= unitAbility.abilityCost * 20f;
                currentCharge = Mathf.Max(0f, currentCharge);
            }
        }

        isCasting = false;
    }

    SpecialAbilityData PickSpecialAbility()
    {
        var abilities = GetCurrentFactionAbilities();

        if (abilities == null || abilities.Count == 0)
            return null;

        abilityIndex = Mathf.Clamp(abilityIndex, 0, abilities.Count - 1);

        SpecialAbilityData ability = abilities[abilityIndex];

        if (abilities.Count > 1)
            abilityIndex = (abilityIndex + 1) % abilities.Count;

        return ability;
    }

    AbilitySO PickUnitAbility()
    {
        CollectEnemyUnitAbilities();

        if (enemyUnitAbilities.Count == 0)
            return null;

        List<AbilitySO> affordableAbilities = new List<AbilitySO>();
        foreach (var ability in enemyUnitAbilities)
        {
            if (currentCharge >= ability.abilityCost * 20f)
                affordableAbilities.Add(ability);
        }

        if (affordableAbilities.Count == 0)
            return null;

        return affordableAbilities[Random.Range(0, affordableAbilities.Count)];
    }

    void CollectEnemyUnitAbilities()
    {
        enemyUnitAbilities.Clear();

        var enemyUnits = GameplayRegistry.GetUnits(Side.Enemy);
        foreach (var unit in enemyUnits)
        {
            if (unit == null || unit.unitProduceSO == null || unit.unitProduceSO.abilities == null)
                continue;

            foreach (var ability in unit.unitProduceSO.abilities)
            {
                if (ability.abilityType == AbilityType.Active && !enemyUnitAbilities.Contains(ability))
                    enemyUnitAbilities.Add(ability);
            }
        }
    }

    List<SpecialAbilityData> GetCurrentFactionAbilities()
    {
        foreach (var set in factionAbilities)
        {
            if (set.faction == GameData.enemyFaction)
                return set.abilities;
        }
        return null;
    }

    Vector3 FindBestTargetPosition(float abilityRadius)
    {
        if (scanCenter == null)
            return transform.position;

        Vector3 bestPos = scanCenter.position;
        int maxCount = 0;

        float innerRadius = abilityRadius * 0.5f;
        float step = innerRadius;

        for (float x = -scanRadius; x <= scanRadius; x += step)
        {
            for (float z = -scanRadius; z <= scanRadius; z += step)
            {
                Vector3 point = scanCenter.position + new Vector3(x, 0f, z);

                int count = CountUnitsInRadius(point, innerRadius);

                //  draw every scan circle
                //DebugDrawCircle(point, innerRadius, Color.red);

                if (count > maxCount)
                {
                    maxCount = count;
                    bestPos = point;
                }
            }
        }

        //  highlight final best target
        //DebugDrawCircle(bestPos, innerRadius, Color.green);

        return bestPos;
    }
    int CountUnitsInRadius(Vector3 center, float radius)
    {
        Collider[] nearby = Physics.OverlapSphere(center, radius, playerLayer);

        int count = 0;
        foreach (var col in nearby)
        {
            if (col.GetComponent<BuildingStats>() == null)
                count++;
        }

        return count;
    }

    void ExecuteSpecialAbility(SpecialAbilityData ability, Vector3 targetPos)
    {
        targetPos += Vector3.up * 0.5f;

        GameObject vfx = Instantiate(ability.vfxPrefab, targetPos, Quaternion.identity);
        Destroy(vfx, 5f);

        Collider[] hits = Physics.OverlapSphere(targetPos, ability.damageArea);

        foreach (Collider col in hits)
        {
            Stats stats = col.GetComponent<Stats>();
            if (stats == null || stats.side != Side.Player)
                continue;
            
            if (ability.type == SpecialAbilityType.Lightning)
            {
                if (col.GetComponent<BuildingStats>() != null)
                    continue;
            }

            float distance = Vector3.Distance(targetPos, col.transform.position);
            float radius = ability.damageArea;
            float baseDamage = ability.damage;

            float finalDamage = 0f;

            if (distance <= radius * 0.4f)
                finalDamage = baseDamage;
            else if (distance <= radius * 0.6f)
                finalDamage = baseDamage * 0.8f;
            else if (distance <= radius * 0.8f)
                finalDamage = baseDamage * 0.4f;
            else if (distance <= radius)
                finalDamage = baseDamage * 0.2f;

            stats.TakeDamage(finalDamage);
        }
    }

    void ExecuteUnitAbility(AbilitySO ability)
    {
        List<AbilityController> targets = GetAbilityTargets(ability);

        foreach (var controller in targets)
        {
            controller.ActivateAbility(ability);
        }
    }

    List<AbilityController> GetAbilityTargets(AbilitySO ability)
    {
        List<AbilityController> targets = new List<AbilityController>();

        List<UnitStats> unitsToCheck = ability.abilityScope == AbilityScope.Personal
            ? GameplayRegistry.GetUnits(Side.Enemy)
            : GameplayRegistry.AllUnits;

        foreach (var unit in unitsToCheck)
        {
            if (unit == null) continue;
            if (!unit.TryGetComponent(out AbilityController controller)) continue;

            switch (ability.targetType)
            {
                case AbilityTargetType.All:
                    targets.Add(controller);
                    break;

                case AbilityTargetType.UnitClass:
                    if (unit.unitType == ability.targetUnitType)
                        targets.Add(controller);
                    break;

                case AbilityTargetType.UnitName:
                    if (unit.gameUnitName == ability.targetUnitName)
                        targets.Add(controller);
                    break;
            }
        }

        return targets;
    }
    
    void DebugDrawCircle(Vector3 center, float radius, Color color)
    {
        center.y += 1.5f;

        int segments = 32;
        float angleStep = 2f * Mathf.PI / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;

            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            Debug.DrawLine(prevPoint, nextPoint, color, 5f, false);

            prevPoint = nextPoint;
        }
    }
}