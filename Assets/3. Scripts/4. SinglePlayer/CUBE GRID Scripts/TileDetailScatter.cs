using UnityEngine;

public class TileDetailScatter : MonoBehaviour
{
    public GameObject[] detailPrefabs;

    public int maxDetailsPerTile = 3;

    public float spawnChance = 0.6f;

    public float tileSize = 1f;

    void Awake()
    {
        ScatterDetails();
    }

    void ScatterDetails()
    {
        int count = Random.Range(0, maxDetailsPerTile + 1);

        for (int i = 0; i < count; i++)
        {
            if (Random.value > spawnChance) continue;

            GameObject prefab = detailPrefabs[Random.Range(0, detailPrefabs.Length)];

            float x = Random.Range(-tileSize / 2f, tileSize / 2f);
            float z = Random.Range(-tileSize / 2f, tileSize / 2f);

            Vector3 pos = transform.position + new Vector3(x, 0.01f, z);

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

            float randomScale = Random.Range(0.8f, 1.2f);
            obj.transform.localScale *= randomScale;

            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }
}