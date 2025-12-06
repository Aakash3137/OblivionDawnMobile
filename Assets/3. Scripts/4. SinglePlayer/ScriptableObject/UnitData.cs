using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Settings")]
    public string unitName;
    public GameObject prefab;   // reference to the unit prefab
    public float buildTime;     // time to produce (seconds)

    [Header("Combat Stats")]
    public int health;
    public int attackPower;
    public float attackRange;   // NEW: range for attacking
    public float attackInterval = 0.5f; // NEW: time between attacks
}
