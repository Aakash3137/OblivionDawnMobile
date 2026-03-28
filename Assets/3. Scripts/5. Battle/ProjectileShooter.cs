using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Weapon")]
    public List<Transform> muzzlePoints = new List<Transform>();
    
    public ProjectileDefinition projectile;
    private float unitDamage;
    private float buildingDamage;

    [Header("Trail Materials")]
    public Material playerTrailMaterial;
    public Material enemyTrailMaterial;

    private Side projectileSide;
    private Stats shooterStats;

    void Start() 
    {
        shooterStats = GetComponent<Stats>();
        projectileSide = shooterStats.side;
    }

    public void Fire(Stats target)
    {
        if (target == null || projectile == null || muzzlePoints.Count == 0)
            return;
        
        // Get current damage values (with modifiers)
        if (TryGetComponent<UnitStats>(out var unitStats))
        {
            if (TryGetComponent<GroundUnit>(out var groundUnit))
            {
                unitDamage = groundUnit.GetModifiedUnitDamage();
                buildingDamage = groundUnit.GetModifiedBuildingDamage();
            }
            else if (TryGetComponent<AirUnit>(out var airUnit))
            {
                unitDamage = airUnit.GetModifiedUnitDamage();
                buildingDamage = airUnit.GetModifiedBuildingDamage();
            }
            else
            {
                unitDamage = unitStats.unitData.unitAttackStats.damage;
                buildingDamage = unitStats.unitData.unitAttackStats.buildingDamage;
            }
        }
        else if(TryGetComponent<DefenseBuildingStats>(out var buildingStats))
        {
            unitDamage = buildingStats.defenseBuildingData.defenseAttackStats.damage;
            buildingDamage = buildingStats.defenseBuildingData.defenseAttackStats.buildingDamage;
        }
        
        int muzzleCount = muzzlePoints.Count;
        
        float dividedUnitDamage = unitDamage / muzzleCount;
        float dividedBuildingDamage = buildingDamage / muzzleCount;
        
        if (projectile.projectileType == ProjectileType.Missile && GetComponent<UnitStats>().unitProduceSO.unitIdentity.faction == FactionName.Futuristic)
        {
            if (projectileSide == Side.Player)
            {
                projectile.projectileType = ProjectileType.Missile_Blue;
            }
            else
            {
                projectile.projectileType = ProjectileType.Missile_Red;
            }
        
        }

        foreach (Transform muzzle in muzzlePoints)
        {
            if (muzzle == null) continue;

            Projectile proj = ProjectilePoolManager.Instance.Get(projectile.projectileType);

            proj.transform.position = muzzle.position;

            Vector3 direction = (target.transform.position - muzzle.position).normalized;
            proj.transform.rotation = Quaternion.LookRotation(direction);

            // SET LAUNCH VFX
            if (shooterStats.side == Side.Player)
            {
                if (projectile.playerLaunchVFX != null)
                    projectile.launchVFX = projectile.playerLaunchVFX;
            }
            else
            {
                if (projectile.enemyLaunchVFX != null)
                    projectile.launchVFX = projectile.enemyLaunchVFX;
            }

            if (projectile.launchVFX != null)
            {
                GameObject vfx = Instantiate(projectile.launchVFX, muzzle.position, proj.transform.rotation);

                if (vfx.TryGetComponent<ProjectileMoveScript>(out var moveScript))
                    moveScript.shooterSide = projectileSide;

                ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.loop = false;

                Destroy(vfx, ps.main.duration);
            }

            proj.gameObject.SetActive(true);

            proj.Init(
                target,
                dividedUnitDamage,
                dividedBuildingDamage,
                projectile,
                GetTrailMaterial(),
                projectileSide,
                shooterStats
            );
        }
    }
    Material GetTrailMaterial()
    {
        return projectileSide == Side.Player ? playerTrailMaterial : enemyTrailMaterial;
    }
}