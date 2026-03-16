using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GroundTile : MonoBehaviour
{
    public int atlasSize = 4;

    // void Start()
    // {
    //     ApplyRandomTile();
    // }

    void Awake()
    {
        ApplyRandomTile();
    }

    void ApplyRandomTile()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector2[] uvs = mesh.uv;

        float tileSize = 1f / atlasSize;

        int index = Random.Range(0, atlasSize * atlasSize);

        int x = index % atlasSize;
        int y = index / atlasSize;

        Vector2 offset = new Vector2(x * tileSize, y * tileSize);

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = (uvs[i] * tileSize) + offset;
        }

        mesh.uv = uvs;
    }
}