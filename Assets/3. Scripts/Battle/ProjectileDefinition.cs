using UnityEngine;

[System.Serializable]
public class ProjectileDefinition
{
    public ProjectileType projectileType;
    public ProjectileMotion motion;

    [Header("Movement")]
    public float speed = 20f;
    public float maxRange = 10f;
    public float arcHeight = 4f;
    public float lifeTime = 5f;

    [Header("Damage")]
    public bool isAreaDamage;
    public float damageRadius = 3f;

    [Header("Targeting")]
    public bool canHitAir;
    public bool canHitGround;

    [Header("Visuals")]
    public bool hasTrail = true;
    public GameObject launchVFX;
    public GameObject hitVFX;
}