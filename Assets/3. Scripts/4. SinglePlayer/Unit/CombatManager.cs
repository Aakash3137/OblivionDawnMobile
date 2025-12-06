using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ResolveCombat(GameObject unitA, GameObject unitB)
    {
        UnitStats statsA = unitA.GetComponent<UnitStats>();
        UnitStats statsB = unitB.GetComponent<UnitStats>();

        if (statsA == null || statsB == null) return;

        // Simple combat: higher attack wins
        if (statsA.attackPower >= statsB.attackPower)
        {
            Destroy(unitB);
            Debug.Log($"{unitA.name} defeated {unitB.name}");
        }
        else
        {
            Destroy(unitA);
            Debug.Log($"{unitB.name} defeated {unitA.name}");
        }
    }
}
