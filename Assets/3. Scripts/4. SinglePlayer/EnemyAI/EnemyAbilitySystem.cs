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

    public LayerMask playerLayer;
    public float scanRadius = 50f;
    public int scanSamples = 20;

    private bool isCasting = false;
    private int abilityIndex = 0;

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

        if (currentCharge >= maxCharge && !isCasting)
        {
            StartCoroutine(CastAbilityRoutine());
        }
    }

    IEnumerator CastAbilityRoutine()
    {
        isCasting = true;

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        SpecialAbilityData ability = PickAbility();
        if (ability == null)
        {
            isCasting = false;
            yield break;
        }

        Vector3 bestTarget = FindBestTargetPosition(ability.damageArea);

        ExecuteAbility(ability, bestTarget);

        currentCharge = 0f;

        isCasting = false;
    }

    SpecialAbilityData PickAbility()
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

    List<SpecialAbilityData> GetCurrentFactionAbilities()
    {
        foreach (var set in factionAbilities)
        {
            if (set.faction == GameData.enemyFaction)
                return set.abilities;
        }
        return null;
    }

    Vector3 FindBestTargetPosition(float radius)
    {
        Collider[] allPlayers = Physics.OverlapSphere(transform.position, scanRadius, playerLayer);

        if (allPlayers.Length == 0)
            return transform.position;

        Vector3 bestPos = transform.position;
        int maxHits = 0;

        foreach (var unit in allPlayers)
        {
            Vector3 center = unit.transform.position;
            int count = CountUnitsInRadius(center, radius);

            if (count > maxHits)
            {
                maxHits = count;
                bestPos = center;
            }
        }

        for (int i = 0; i < scanSamples; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * scanRadius;
            randomPoint.y = transform.position.y;

            int count = CountUnitsInRadius(randomPoint, radius);

            if (count > maxHits)
            {
                maxHits = count;
                bestPos = randomPoint;
            }
        }

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

    void ExecuteAbility(SpecialAbilityData ability, Vector3 targetPos)
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
}