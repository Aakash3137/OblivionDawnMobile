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

        // Apply passive abilities immediately
        foreach (var ability in abilities)
        {
            if (ability.abilityType == AbilityType.Passive)
            {
                ApplyAbility(ability);
            }
        }
    }
    
    public void ActivateAbility(AbilitySO ability)
    {
        ApplyAbility(ability);
    }

    private void ApplyAbility(AbilitySO ability)
    {
        foreach (var effect in ability.effects)
        {
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