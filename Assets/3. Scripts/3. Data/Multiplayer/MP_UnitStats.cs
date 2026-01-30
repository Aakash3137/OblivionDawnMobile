using UnityEngine;

[CreateAssetMenu(menuName = "Multiplayer/Stats/Unit")]
public class MP_UnitStats : ScriptableObject
{
    public int maxHealth;
    public int damage;
    public float moveSpeed;
    public float attackRate;
}