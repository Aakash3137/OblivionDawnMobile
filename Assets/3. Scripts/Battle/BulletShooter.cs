using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public Transform muzzlePoint;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 3f;

    public void Fire(Transform target, float damage)
    {
        if (target == null) return;

        Bullet bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = muzzlePoint.position;
        bullet.transform.rotation = Quaternion.identity;

        bullet.gameObject.SetActive(true);
        bullet.Init(target, bulletSpeed, damage, bulletLifeTime);
    }
}