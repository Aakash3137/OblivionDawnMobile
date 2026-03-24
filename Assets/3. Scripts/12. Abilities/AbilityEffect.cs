using UnityEngine;

public abstract class AbilityEffect : ScriptableObject, IAbilityEffect
{
    public abstract void Apply(Stats target);
    public abstract void Remove(Stats target);
}