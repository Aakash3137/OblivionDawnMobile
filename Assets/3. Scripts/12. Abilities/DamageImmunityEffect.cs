using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Damage Immunity")]
public class DamageImmunityEffect : AbilityEffect
{
    public override void Apply(Stats target)
    {
        target.SetDamageImmunity(true);
        Debug.Log($"[DamageImmunity] {target.gameObject.name} is now immune to damage");
    }

    public override void Remove(Stats target)
    {
        target.SetDamageImmunity(false);
        Debug.Log($"[DamageImmunity] {target.gameObject.name} immunity removed");
    }
}
