using UnityEngine;

[CreateAssetMenu(menuName = "Multiplayer/Stats/Building")]
public class MP_BuildingStats : ScriptableObject
{
    public int maxHealth;
    public int spawningSpeed;
    public int damage;
    public float attackRate;
    public float range;
}
