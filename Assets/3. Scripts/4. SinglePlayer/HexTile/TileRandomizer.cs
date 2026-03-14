using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AtlasTileRandomizer : MonoBehaviour
{
    public int atlasSize = 3; // 3x3 texture atlas

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        float tileSize = 1f / atlasSize;

        // Pick a random tile from the atlas
        int x = Random.Range(0, atlasSize);
        int y = Random.Range(0, atlasSize);

        Vector2 scale = new Vector2(tileSize, tileSize);
        Vector2 offset = new Vector2(x * tileSize, y * tileSize);

        rend.material.mainTextureScale = scale;
        rend.material.mainTextureOffset = offset;
    }
}