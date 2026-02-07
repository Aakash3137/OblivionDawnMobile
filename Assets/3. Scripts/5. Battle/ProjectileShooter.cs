using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Weapon")]
    public Transform muzzlePoint;
    public ProjectileDefinition projectile;
    public float damage = 1f;

    [Header("Trail Materials")]
    public Material playerTrailMaterial;
    public Material enemyTrailMaterial;

    private Side projectileSide;

    void Start()
    {
        projectileSide = GetComponent<Stats>().side;
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

        proj.Init(target, damage, projectile, GetTrailMaterial(), projectileSide);
    }

    Material GetTrailMaterial()
    {
        return projectileSide == Side.Player ? playerTrailMaterial : enemyTrailMaterial;
    }
}