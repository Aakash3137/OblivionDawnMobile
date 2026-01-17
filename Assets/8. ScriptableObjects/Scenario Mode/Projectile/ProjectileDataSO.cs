using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Data", menuName = "Scenario Stats/Projectile Data")]
public class ProjectileDataSO : ScriptableObject
{
    public string projectileName;
    public ProjectileType projectileType;
    public ProjectileMotion projectileMotion;
    public GameObject projectilePrefab;
    public ProjectileBasicStats projectileBasicStats;
    public ProjectileVisuals projectileVisuals;
    public ProjectileAOE projectileAOE;

    private void Validate()
    {
        projectileBasicStats.projectileSpeed = Mathf.Max(projectileBasicStats.projectileSpeed, 0);
        projectileBasicStats.projectileRange = Mathf.Max(projectileBasicStats.projectileRange, 0);
        projectileBasicStats.projectileLifetime = Mathf.Max(projectileBasicStats.projectileLifetime, 0);
        projectileBasicStats.projectileMaxArcHeight = Mathf.Max(projectileBasicStats.projectileMaxArcHeight, 0);
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
    public float projectileSpeed;
    public float projectileRange;
    public float projectileLifetime;
    public float projectileMaxArcHeight;
}

[Serializable]
public struct ProjectileVisuals
{
    public bool hasTail;
    public GameObject projectileVFX;
    public TrailRenderer projectileTrail;
}

[Serializable]
public struct ProjectileAOE
{
    public bool isAOE;
    public float aoeRadius;
}