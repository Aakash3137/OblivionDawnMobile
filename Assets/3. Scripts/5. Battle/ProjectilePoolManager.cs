using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public ProjectileType type;
        public Projectile prefab;
        public int size = 20;
    }

    [SerializeField] private List<Pool> pools;

    private Dictionary<ProjectileType, List<Projectile>> runtimePools = new();

    void Awake()
    {
        Instance = this;

        foreach (var pool in pools)
        {
            List<Projectile> list = new();

            for (int i = 0; i < pool.size; i++)
            {
                Projectile p = Instantiate(pool.prefab, transform.position, Quaternion.identity, transform);
                p.gameObject.SetActive(false);
                list.Add(p);
            }

            runtimePools.Add(pool.type, list);
        }
    }

    public Projectile Get(ProjectileType type)
    {
        foreach (Projectile p in runtimePools[type])
        {
            if (!p.gameObject.activeInHierarchy)
                return p;
        }

        // Expand pool if needed
        Projectile extra = Instantiate(runtimePools[type][0], transform);
        extra.gameObject.SetActive(false);
        runtimePools[type].Add(extra);
        return extra;
    }
}