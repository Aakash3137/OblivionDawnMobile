using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilitySystem : MonoBehaviour
{
    [Header("Abilities")]
    public List<SpecialAbilityData> specialAbilities;

    [Header("Charge Settings")]
    public float currentCharge = 0f;
    public float maxCharge = 100f;
    public float chargePerKill = 5f;

    [Header("Targeting")]
    public LayerMask playerLayer;
    public float scanRadius = 50f;
    public int scanSamples = 20; // how many points AI checks

    private bool isCasting = false;

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
        Debug.Log("Enemy casting ability!");
        isCasting = true;

        yield return new WaitForSeconds(Random.Range(1f, 2f)); // small delay (feels human)

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
        Debug.Log("Enemy Picking ability!");
        if (specialAbilities.Count == 0) return null;

        return specialAbilities[Random.Range(0, specialAbilities.Count)];
    }

    Vector3 FindBestTargetPosition(float radius)
    {
        Debug.Log("Enemy finding target for Ability usage! ");
        Collider[] allPlayers = Physics.OverlapSphere(transform.position, scanRadius, playerLayer);

        if (allPlayers.Length == 0)
            return transform.position;

        Vector3 bestPos = allPlayers[0].transform.position;
        int maxHits = 0;

        foreach (var unit in allPlayers)
        {
            Vector3 center = unit.transform.position;
            Collider[] nearby = Physics.OverlapSphere(center, radius, playerLayer);
        
            // Count only units, not buildings
            int count = 0;
            foreach (var col in nearby)
            {
                if (col.GetComponent<BuildingStats>() == null) // Exclude buildings
                    count++;
            }

            if (count > maxHits)
            {
                maxHits = count;
                bestPos = center;
            }
        }

        return bestPos;
    }

    void ExecuteAbility(SpecialAbilityData ability, Vector3 targetPos)
    {
        Debug.Log("Enemy Executing Ability! ");
        targetPos += Vector3.up * 0.5f;

        // VFX
        GameObject vfx = Instantiate(ability.vfxPrefab, targetPos, Quaternion.identity);
        Destroy(vfx, 5f);

        // Damage
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

        Debug.Log($"Enemy used {ability.type} at {targetPos}");
    }
}