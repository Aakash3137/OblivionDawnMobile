using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Speed Modifier")]
public class SpeedModifierEffect : AbilityEffect
{
    public float speedAmount; // e.g. +1 or +2

    public override void Apply(Stats target)
    {
        if (!target.TryGetComponent<GroundUnit>(out var unit))
            return;

        unit.AddSpeedModifier(this, speedAmount);
    }

    public override void Remove(Stats target)
    {
        if (!target.TryGetComponent<GroundUnit>(out var unit))
            return;

        unit.RemoveSpeedModifier(this);
    }
}