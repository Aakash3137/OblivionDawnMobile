using UnityEngine;

public enum StatModifierType
{
    HealthToFull,
    UnitDamageIncrease,
    BuildingDamageIncrease
}

[CreateAssetMenu(menuName = "Abilities/Effects/Stat Modifier")]
public class StatModifierEffect : AbilityEffect
{
    public StatModifierType modifierType;
    [Header("UnitDamageIncrease, BuildingDamageIncrease Only" )]
    public float increaseAmount;

    public override void Apply(Stats target)
    {
        switch (modifierType)
        {
            case StatModifierType.HealthToFull:
                target.ResetHealth();
                Debug.Log($"[StatModifier] {target.gameObject.name} health restored to full");
                break;

            case StatModifierType.UnitDamageIncrease:
                if (target.TryGetComponent<GroundUnit>(out var groundUnit))
                {
                    groundUnit.AddDamageModifier(this, increaseAmount, true);
                }
                else if (target.TryGetComponent<AirUnit>(out var airUnit))
                {
                    airUnit.AddDamageModifier(this, increaseAmount, true);
                }
                break;

            case StatModifierType.BuildingDamageIncrease:
                if (target.TryGetComponent<GroundUnit>(out var groundUnit2))
                {
                    groundUnit2.AddDamageModifier(this, increaseAmount, false);
                }
                else if (target.TryGetComponent<AirUnit>(out var airUnit2))
                {
                    airUnit2.AddDamageModifier(this, increaseAmount, false);
                }
                break;
        }
    }

    public override void Remove(Stats target)
    {
        switch (modifierType)
        {
            case StatModifierType.UnitDamageIncrease:
            case StatModifierType.BuildingDamageIncrease:
                if (target.TryGetComponent<GroundUnit>(out var groundUnit))
                {
                    groundUnit.RemoveDamageModifier(this);
                }
                else if (target.TryGetComponent<AirUnit>(out var airUnit))
                {
                    airUnit.RemoveDamageModifier(this);
                }
                break;
        }
    }
}
