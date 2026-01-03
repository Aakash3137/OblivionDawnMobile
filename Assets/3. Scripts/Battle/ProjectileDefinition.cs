using UnityEngine;

[System.Serializable]
public class ProjectileDefinition
{
    public ProjectileType projectileType;
    public ProjectileMotion motion;

    public float speed = 20f;
    public float lifeTime = 3f;
    public bool hasTrail = true;

    // Future VFX
    public GameObject launchVFX;
    public GameObject hitVFX;
}