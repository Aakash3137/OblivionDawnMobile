using UnityEngine;

public class UnitHealth : Health
{
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} unit died!");
        base.Die();
    }
}
