using UnityEngine;

public class MP_Building : MonoBehaviour
{
    int health;
    int damage;
    int spawningspeed;
    public void Initialize(MP_BuildingStats stats)
    {
        health = stats.maxHealth;
        damage = stats.damage;
        spawningspeed = stats.spawningSpeed;
    }
}