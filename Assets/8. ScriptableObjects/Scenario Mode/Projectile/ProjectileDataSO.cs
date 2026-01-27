using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Data", menuName = "Scenario Stats/Projectile Data")]
public class ProjectileDataSO : ScriptableObject
{
    public string projectileName;
    public ProjectileType projectileType;
    public ProjectileMotion projectileMotion;
    public ProjectileBasicStats projectileBasicStats;
    public ProjectileVisuals projectileVisuals;
    public ProjectileAOE projectileAOE;

    private void Validate()
    {
        projectileBasicStats.speed = Mathf.Max(projectileBasicStats.speed, 0);
        projectileBasicStats.maxRange = Mathf.Max(projectileBasicStats.maxRange, 0);
        projectileBasicStats.lifeTime = Mathf.Max(projectileBasicStats.lifeTime, 0);
        projectileBasicStats.maxArcHeight = Mathf.Max(projectileBasicStats.maxArcHeight, 0);
        projectileAOE.aoeRadius = Mathf.Max(projectileAOE.aoeRadius, 0);
    }

    private void OnValidate()
    {
        Validate();
    }
}


[Serializable]
public struct ProjectileBasicStats
{
    public bool isRayCast;
    public float speed;
    public float maxRange;
    public float lifeTime;
    public float maxArcHeight;
}

[Serializable]
public struct ProjectileVisuals
{
    public bool hasTrail;
    public GameObject hitVFX;
}

[Serializable]
public struct ProjectileAOE
{
    public bool isAOE;
    public float aoeRadius;
}