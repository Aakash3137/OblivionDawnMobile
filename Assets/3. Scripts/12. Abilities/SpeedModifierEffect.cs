using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Speed Modifier")]
public class SpeedModifierEffect : AbilityEffect
{
    public float speedMultiplier; 

    public override void Apply(Stats target)
    {

        if (!target.TryGetComponent<GroundUnit>(out var unit))
        {
            return;
        }

        unit.AddSpeedMultiplier(this, speedMultiplier);
    }

    public override void Remove(Stats target)
    {

        if (!target.TryGetComponent<GroundUnit>(out var unit))
        {
            return;
        }

        unit.RemoveSpeedModifier(this);
    }
}