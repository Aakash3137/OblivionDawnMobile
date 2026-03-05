using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon")]
    public Transform muzzlePoint;
    public ProjectileDataSO projectileData;

    public void Fire(Stats target, float unitdamage, float buildingDamage, Side side)
    {
        if (target == null)
            return;

        Projectile proj = ProjectilePoolManager.Instance.Get(projectileData.projectileType);

        proj.transform.position = muzzlePoint.position;
        proj.transform.rotation = gameObject.transform.rotation;
        proj.gameObject.SetActive(true);

        proj.Initialize(target, unitdamage, buildingDamage, projectileData, side);
    }

}
