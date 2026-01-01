using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int poolSize = 30;

    private List<Bullet> pool = new List<Bullet>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            Bullet b = Instantiate(bulletPrefab, transform);
            b.gameObject.SetActive(false);
            pool.Add(b);
        }
    }

    public Bullet GetBullet()
    {
        foreach (Bullet b in pool)
        {
            if (!b.gameObject.activeInHierarchy)
                return b;
        }

        Bullet extra = Instantiate(bulletPrefab, transform);
        extra.gameObject.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}