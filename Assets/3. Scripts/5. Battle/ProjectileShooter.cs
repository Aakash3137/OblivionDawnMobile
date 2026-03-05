using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Weapon")]
    public Transform muzzlePoint;
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
        unitDamage = GetComponent<UnitStats>().unitData.unitAttackStats.damage;
        buildingDamage = GetComponent<UnitStats>().unitData.unitAttackStats.buildingDamage;
        projectileSide = shooterStats.side;
    }

    public void Fire(Stats target)
    {
        if (target == null || projectile == null)
            return;

        Projectile proj =
            ProjectilePoolManager.Instance.Get(projectile.projectileType);

        proj.transform.position = muzzlePoint.position;
        
        Vector3 direction = (target.transform.position - muzzlePoint.position).normalized;
        proj.transform.rotation = Quaternion.LookRotation(direction);

        //  LAUNCH VFX
        if (projectile.launchVFX != null && muzzlePoint != null)
        {
            GameObject vfx = Instantiate(projectile.launchVFX, muzzlePoint.position, proj.transform.rotation, gameObject.transform);
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = false;
            Destroy(vfx, ps.main.duration);
        }
        proj.gameObject.SetActive(true);
        
        proj.Init(target, unitDamage, buildingDamage, projectile, GetTrailMaterial(), projectileSide, shooterStats);
    }

    Material GetTrailMaterial()
    {
        return projectileSide == Side.Player ? playerTrailMaterial : enemyTrailMaterial;
    }
}