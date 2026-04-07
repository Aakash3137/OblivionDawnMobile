using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Speed Modifier")]
public class SpeedModifierEffect : AbilityEffect
{
    public float speedMultiplier; 

    public override void Apply(Stats target)
    {
        if (target.TryGetComponent<GroundUnit>(out var groundUnit))
        {
            groundUnit.AddSpeedMultiplier(this, speedMultiplier);
            return;
        }

        if (target.TryGetComponent<AirUnit>(out var airUnit))
        {
            airUnit.AddSpeedMultiplier(this, speedMultiplier);
            return;
        }
    }

    public override void Remove(Stats target)
    {
        if (target.TryGetComponent<GroundUnit>(out var groundUnit))
        {
            groundUnit.RemoveSpeedModifier(this);
            return;
        }

        if (target.TryGetComponent<AirUnit>(out var airUnit))
        {
            airUnit.RemoveSpeedModifier(this);
            return;
        }
    }
}