/*using Fusion;
using UnityEngine;

public class NetworkCombatManager : NetworkBehaviour
{
    public static NetworkCombatManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Only server resolves combat
    public void ResolveCombat(NetworkObject attacker, NetworkObject defender)
    {
        if (!Object.HasStateAuthority)
            return;

        var statsA = attacker.GetComponent<UnitStats>();
        var statsB = defender.GetComponent<UnitStats>();

        if (statsA == null || statsB == null) return;

        // Simple combat logic: higher attack wins
        if (statsA.attackPower >= statsB.attackPower)
        {
            DespawnUnit(defender);
            Debug.Log($"{attacker.name} defeated {defender.name}");
        }
        else
        {
            DespawnUnit(attacker);
            Debug.Log($"{defender.name} defeated {attacker.name}");
        }
    }

    private void DespawnUnit(NetworkObject unit)
    {
        if (unit != null && unit.HasStateAuthority)
            unit.Despawn();
    }
}
*/