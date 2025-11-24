using UnityEngine;

[ExecuteInEditMode]   // runs in editor, even when not playing
public class HexSnap : MonoBehaviour
{
    [Header("Hex Settings")]
    public float hexSize = 1f;       // radius of hex tile
    public bool pointyTop = true;    // orientation: pointy or flat

    void Update()
    {
        if (!Application.isPlaying)
        {
            Vector3 pos = transform.position;
            Vector2Int hexCoord = WorldToHex(pos);
            transform.position = HexToWorld(hexCoord);
        }
    }

    // Convert world position → hex coordinates
    private Vector2Int WorldToHex(Vector3 position)
    {
        if (pointyTop)
        {
            float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexSize;
            float r = (2f / 3f * position.z) / hexSize;
            return HexRound(new Vector2(q, r));
        }
        else
        {
            float q = (2f / 3f * position.x) / hexSize;
            float r = (-1f / 3f * position.x + Mathf.Sqrt(3f) / 3f * position.z) / hexSize;
            return HexRound(new Vector2(q, r));
        }
    }

    // Round to nearest hex
    private Vector2Int HexRound(Vector2 hex)
    {
        int q = Mathf.RoundToInt(hex.x);
        int r = Mathf.RoundToInt(hex.y);
        return new Vector2Int(q, r);
    }

    // Convert hex coordinates → world position
    private Vector3 HexToWorld(Vector2Int hex)
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
