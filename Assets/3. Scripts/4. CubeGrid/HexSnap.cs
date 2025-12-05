using UnityEngine;

[ExecuteAlways]
public class HexSnap : MonoBehaviour
{
    private TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        // Read settings from HexGridManager
        var gm = CubeGridManager.Instance;
        if (gm == null) return;

        float hexSize = gm.hexSize;
        bool pointyTop = gm.pointyTop;

        // Snap using the same formulas as HexGridManager
        Vector3 pos = transform.position;
        Vector2Int hexCoord = gm.WorldToHex(pos);
        transform.position = PointyOrFlatHexToWorld(hexCoord, hexSize, pointyTop);

        string label = $"({hexCoord.x}, {hexCoord.y})";
        if (textMesh != null) textMesh.text = label;
        gameObject.name = label;
    }

    private Vector3 PointyOrFlatHexToWorld(Vector2Int hex, float hexSize, bool pointyTop)
    {
        if (pointyTop)
        {
            float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
            float z = hexSize * 3f / 2f * hex.y;
            return new Vector3(x, 0, z);
        }
        else
        {
            float x = hexSize * 3f / 2f * hex.x;
            float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
            return new Vector3(x, 0, z);
        }
    }
}





// using UnityEngine;

// [ExecuteAlways]   // runs in editor AND play mode
// public class HexSnap : MonoBehaviour
// {
//     [Header("Hex Settings")]
//     public float hexSize = 1f;       // radius of hex tile
//     public bool pointyTop = true;    // orientation: pointy or flat

//     private TextMesh textMesh;       // reference to child TextMesh

//     void OnEnable() // safer than Awake in edit mode
//     {
//         textMesh = GetComponentInChildren<TextMesh>();
//     }

//     void Update()
//     {
//         Vector3 pos = transform.position;
//         Vector2Int hexCoord = WorldToHex(pos);
//         transform.position = HexToWorld(hexCoord);

//         string label = $"({hexCoord.x}, {hexCoord.y})";
//         if (textMesh != null) textMesh.text = label;
//         gameObject.name = label;
//     }

//     // Convert world position → hex coordinates (axial q,r)
//     public Vector2Int WorldToHex(Vector3 position)
//     {
//         if (pointyTop)
//         {
//             float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexSize;
//             float r = (2f / 3f * position.z) / hexSize;
//             return HexRound(q, r);
//         }
//         else
//         {
//             float q = (2f / 3f * position.x) / hexSize;
//             float r = (-1f / 3f * position.x + Mathf.Sqrt(3f) / 3f * position.z) / hexSize;
//             return HexRound(q, r);
//         }
//     }

//     // Proper cube rounding
//     private Vector2Int HexRound(float q, float r)
//     {
//         float s = -q - r;

//         int rq = Mathf.RoundToInt(q);
//         int rr = Mathf.RoundToInt(r);
//         int rs = Mathf.RoundToInt(s);

//         float qDiff = Mathf.Abs(rq - q);
//         float rDiff = Mathf.Abs(rr - r);
//         float sDiff = Mathf.Abs(rs - s);

//         if (qDiff > rDiff && qDiff > sDiff)
//         {
//             rq = -rr - rs;
//         }
//         else if (rDiff > sDiff)
//         {
//             rr = -rq - rs;
//         }

//         return new Vector2Int(rq, rr);
//     }

//     // Convert hex coordinates → world position
//     private Vector3 HexToWorld(Vector2Int hex)
//     {
//         if (pointyTop)
//         {
//             float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
//             float z = hexSize * 3f / 2f * hex.y;
//             return new Vector3(x, 0, z);
//         }
//         else
//         {
//             float x = hexSize * 3f / 2f * hex.x;
//             float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
//             return new Vector3(x, 0, z);
//         }
//     }
// }
