using UnityEngine;

public class BuildingHealth : Health
{
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} building destroyed!");
        base.Die();
    }
}
