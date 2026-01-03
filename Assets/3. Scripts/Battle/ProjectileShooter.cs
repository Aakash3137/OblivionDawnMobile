using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Weapon")]
    public Transform muzzlePoint;
    public ProjectileDefinition projectile;
    public float damage = 10f;

    [Header("Trail Materials")]
    public Material playerTrailMaterial;
    public Material enemyTrailMaterial;

    private SideScenario sideScenario;

    void Awake()
    {
        sideScenario = GetComponent<SideScenario>();
    }

    public void Fire(BattleUnit target)
    {
        if (target == null) return;

        // 🔥 LAUNCH VFX
        if (projectile.launchVFX != null && muzzlePoint != null)
        {
            Instantiate(
                projectile.launchVFX,
                muzzlePoint.position,
                muzzlePoint.rotation
            );
        }
        
        Projectile proj =
            ProjectilePoolManager.Instance.Get(projectile.projectileType);

        proj.transform.position = muzzlePoint.position;
        proj.transform.rotation = Quaternion.identity;
        proj.gameObject.SetActive(true);

        proj.Init(
            target,
            damage,
            projectile,
            GetTrailMaterial()
        );
    }

    Material GetTrailMaterial()
    {
        if (sideScenario == null)
            return null;

        return sideScenario.side == Side.Player
            ? playerTrailMaterial
            : enemyTrailMaterial;
    }
}