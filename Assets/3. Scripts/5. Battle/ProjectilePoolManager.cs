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

    // ✅ Added pointer per projectile type (minimal addition)
    private Dictionary<ProjectileType, int> nextIndex = new();

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

            // ✅ initialize round-robin pointer
            nextIndex.Add(pool.type, 0);
        }
    }

    public Projectile Get(ProjectileType type)
    {
        List<Projectile> list = runtimePools[type];

        int startIndex = nextIndex[type];

        // ✅ Round-robin search instead of first-free
        for (int i = 0; i < list.Count; i++)
        {
            int index = (startIndex + i) % list.Count;

            if (!list[index].gameObject.activeInHierarchy)
            {
                nextIndex[type] = (index + 1) % list.Count;
                return list[index];
            }
        }

        // Expand pool if needed (same behavior as before)
        Projectile extra = Instantiate(list[0], transform);
        extra.gameObject.SetActive(false);
        list.Add(extra);

        nextIndex[type] = list.Count % list.Count;

        return extra;
    }
}