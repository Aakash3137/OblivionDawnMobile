using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Stats owner;
    private List<AbilitySO> abilities;
    private void Awake()
    {
        owner = GetComponent<Stats>();
    }

    public void Initialize(List<AbilitySO> abilityList)
    {
        abilities = abilityList;

        if (abilities == null || abilities.Count == 0)
        {
            Debug.Log($"[AbilityController] {gameObject.name} has no abilities to initialize");
            return;
        }

        //Debug.Log($"[AbilityController] Initializing {abilities.Count} abilities for {gameObject.name}");

        // Apply passive abilities immediately
        foreach (var ability in abilities)
        {
            if (ability.abilityType == AbilityType.Passive)
            {
                Debug.Log($"[AbilityController] Applying passive ability: {ability.abilityName}");
                ApplyAbility(ability);
            }
        }
    }
    
    public void ActivateAbility(AbilitySO ability)
    {
       // Debug.Log($"[AbilityController] Activating ability {ability.abilityName} on {gameObject.name}");
        ApplyAbility(ability);
    }

    private void ApplyAbility(AbilitySO ability)
    {
        if (ability.effects == null || ability.effects.Count == 0)
        {
            Debug.LogWarning($"[AbilityController] Ability {ability.abilityName} has no effects!");
            return;
        }

       // Debug.Log($"[AbilityController] Applying {ability.effects.Count} effects from {ability.abilityName}");

        foreach (var effect in ability.effects)
        {
            if (effect == null)
            {
                Debug.LogWarning($"[AbilityController] Null effect in {ability.abilityName}");
                continue;
            }

           // Debug.Log($"[AbilityController] Applying effect: {effect.GetType().Name}");
            effect.Apply(owner);
        }

        if (ability.duration > 0)
        {
            StartCoroutine(RemoveAfterDuration(ability));
        }
    }

    private IEnumerator RemoveAfterDuration(AbilitySO ability)
    {
        yield return new WaitForSeconds(ability.duration);

        foreach (var effect in ability.effects)
        {
            effect.Remove(owner);
        }
    }
}