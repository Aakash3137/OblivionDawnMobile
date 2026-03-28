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

    private void Start()
    {
        Generate();
    }

    void Generate()
    {
        Vector2Int playerCity = GameManager.Instance.playerSpawnCoord;
        Vector2Int enemyCity = GameManager.Instance.enemySpawnCoord;

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

    void GenerateSingleCluster(VariationData variation, Vector2Int playerCity, Vector2Int enemyCity)
    {
        int width = Mathf.Clamp(variation.tileCount / 3, 3, 8);
        int height = Mathf.Clamp(variation.tileCount / width, 2, 6);

        int safety = 0;

        while (safety < 300)
        {
            safety++;

            int startX = Random.Range(0, GRID_SIZE);
            int startY = Random.Range(0, GRID_SIZE);

            int placed = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (placed >= variation.tileCount)
                        break;

                    int px = startX + x;
                    int py = startY + y;

                    // 🔥 Allow clipping at edges instead of rejecting
                    if (px < 0 || py < 0 || px >= GRID_SIZE || py >= GRID_SIZE)
                        continue;

                    var coord = new Vector2Int(px, py);

                    if (usedTiles.Contains(coord))
                        continue;

                    if (!IsValid(coord, playerCity, enemyCity))
                        continue;

                    ApplyVariation(coord, variation);
                    placed++;
                }
            }

            if (placed > variation.tileCount * 0.6f)
                break; // accept even partial clusters near edges
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

            // int startX = Random.Range(0, GRID_SIZE - width);
            // int startY = Random.Range(0, GRID_SIZE - height);

            int startX = Random.Range(0, GRID_SIZE);
            int startY = Random.Range(0, GRID_SIZE);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int px = startX + x;
                    int py = startY + y;

                    if (px < 0 || py < 0 || px >= GRID_SIZE || py >= GRID_SIZE)
                        continue;

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
        Tile tile = CubeGridManager.Instance.GetTile(coord);
        if (tile == null) return;

        tile.transform.position += Vector3.down * 0.5f; // sink the tile slightly for better visuals

        tile.GetComponentInChildren<MeshRenderer>().material = variation.material;
        tile.ChangeSide(Side.Neutral);      // for water/lava

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
        int dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        return dist > 3;
        // return Mathf.Abs(a.x - b.x) > 2 && Mathf.Abs(a.y - b.y) > 2;
    }
}