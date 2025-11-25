using System.Collections.Generic; // V.3
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;

    [Header("Grid settings (match HexSnap)")]
    public float hexSize = 1f;
    public bool pointyTop = true;

    private Dictionary<Vector2Int, GameObject> hexMap = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        Instance = this;
        BuildMap();
    }

    // Call this if tiles change in editor or runtime
    public void BuildMap()
    {
        hexMap.Clear();
        foreach (Transform child in transform)
        {
            // Accept both parent tile or nested child
            HexSnap snap = child.GetComponent<HexSnap>();
            if (snap != null)
            {
                Vector2Int coord = WorldToHex(child.position);
                if (!hexMap.ContainsKey(coord))
                    hexMap.Add(coord, child.gameObject);
            }
        }
    }

    public GameObject GetHex(Vector2Int coord)
    {
        hexMap.TryGetValue(coord, out GameObject hex);
        return hex;
    }

    public Vector2Int WorldToHex(Vector3 position)
    {
        if (pointyTop)
        {
            float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexSize;
            float r = (2f / 3f * position.z) / hexSize;
            return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
        }
        else
        {
            float q = (2f / 3f * position.x) / hexSize;
            float r = (-1f / 3f * position.x + Mathf.Sqrt(3f) / 3f * position.z) / hexSize;
            return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
        }
    }

    public Vector3 HexToWorld(Vector2Int hex)
    {
        if (pointyTop)
        {
            float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
            float z = hexSize * 3f / 2f * hex.y;
            return new Vector3(x, 0f, z);
        }
        else
        {
            float x = hexSize * 3f / 2f * hex.x;
            float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
            return new Vector3(x, 0f, z);
        }
    }

    // Optional: find nearest existing hex to a world position
    public Vector2Int NearestExistingHex(Vector3 position)
    {
        Vector2Int approx = WorldToHex(position);
        if (hexMap.ContainsKey(approx)) return approx;

        // search local neighborhood
        Vector2Int[] dirs = {
            new Vector2Int(+1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, +1), new Vector2Int(0, -1),
            new Vector2Int(+1, -1), new Vector2Int(-1, +1)
        };
        for (int ring = 1; ring <= 3; ring++)
        {
            foreach (var d in dirs)
            {
                Vector2Int cand = approx + ring * d;
                if (hexMap.ContainsKey(cand)) return cand;
            }
        }
        // fallback: any tile
        foreach (var kvp in hexMap) return kvp.Key;
        return approx;
    }
}
