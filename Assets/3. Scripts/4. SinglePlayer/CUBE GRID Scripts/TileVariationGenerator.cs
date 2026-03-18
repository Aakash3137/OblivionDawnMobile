using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TileVariationGenerator : MonoBehaviour
{
    [System.Serializable]
    public class VariationData
    {
        public string name; // "Water", "Lava"
        public Material material;
        public int tileCount;
        public bool isSingleCluster; // 🔥 for water
        public bool addNavMeshObstacle;
    }

    public List<VariationData> variations;

    private const int GRID_SIZE = 17;

    private HashSet<Vector2Int> usedTiles = new HashSet<Vector2Int>();

    async void Start()
    {
        await Awaitable.NextFrameAsync();
        await Awaitable.NextFrameAsync();

        Generate();
    }

    void Generate()
    {
        Vector2Int playerCity = GetCoord(GameManager.Instance.playerTile);
        Vector2Int enemyCity = GetCoord(GameManager.Instance.enemyTile);

        foreach (var variation in variations)
        {
            if (variation.isSingleCluster)
            {
                GenerateSingleCluster(variation, playerCity, enemyCity);
            }
            else
            {
                GenerateMultiCluster(variation, playerCity, enemyCity);
            }
        }
    }

    // 🔥 WATER / BIG CLUSTER
    void GenerateSingleCluster(VariationData variation, Vector2Int playerCity, Vector2Int enemyCity)
    {
        int width = Mathf.Clamp(variation.tileCount / 3, 3, 8);
        int height = Mathf.Clamp(variation.tileCount / width, 2, 6);

        int safety = 0;

        while (safety < 200)
        {
            safety++;

            int startX = Random.Range(0, GRID_SIZE - width);
            int startY = Random.Range(0, GRID_SIZE - height);

            List<Vector2Int> cluster = new List<Vector2Int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var coord = new Vector2Int(startX + x, startY + y);

                    if (!IsValid(coord, playerCity, enemyCity))
                        continue;

                    cluster.Add(coord);
                }
            }

            if (cluster.Count < variation.tileCount * 0.7f)
                continue; // retry if too many invalid tiles

            int placed = 0;

            foreach (var coord in cluster)
            {
                if (placed >= variation.tileCount)
                    break;

                if (usedTiles.Contains(coord))
                    continue;

                ApplyVariation(coord, variation);
                placed++;
            }

            break; // only ONE cluster
        }
    }

    // 🔹 LAVA / SMALL PATCHES
    void GenerateMultiCluster(VariationData variation, Vector2Int playerCity, Vector2Int enemyCity)
    {
        int placed = 0;
        int safety = 0;

        while (placed < variation.tileCount && safety < 500)
        {
            safety++;

            int width = Random.Range(2, 4);
            int height = Random.Range(2, 3);

            int startX = Random.Range(0, GRID_SIZE - width);
            int startY = Random.Range(0, GRID_SIZE - height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (placed >= variation.tileCount)
                        return;

                    var coord = new Vector2Int(startX + x, startY + y);

                    if (usedTiles.Contains(coord))
                        continue;

                    if (!IsValid(coord, playerCity, enemyCity))
                        continue;

                    ApplyVariation(coord, variation);
                    placed++;
                }
            }
        }
    }

    void ApplyVariation(Vector2Int coord, VariationData variation)
    {
        Tile tile = CubeGridManager.Instance.GetCube(coord);
        if (tile == null) return;

        tile.tileRenderer.material = variation.material;
        tile.ownerSide = Side.NeutralEnemy; // for water/lava
        
        usedTiles.Add(coord);

        // 🔥 ENABLE EXISTING NAVMESH OBSTACLE
        if (variation.addNavMeshObstacle)
        {
            var obstacle = tile.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacle != null)
            {
                obstacle.enabled = true;
                obstacle.carving = true; // important for runtime updates
            }
            else
            {
                Debug.LogWarning($"NavMeshObstacle missing on tile: {coord}");
            }
        }
    }

    Vector2Int GetCoord(Tile tile)
    {
        return CubeGridManager.Instance.WorldToGrid(tile.transform.position);
    }

    bool IsValid(Vector2Int coord, Vector2Int playerCity, Vector2Int enemyCity)
    {
        if (!FarFromCity(coord, playerCity)) return false;
        if (!FarFromCity(coord, enemyCity)) return false;

        return true;
    }

    bool FarFromCity(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) > 2 && Mathf.Abs(a.y - b.y) > 2;
    }
}