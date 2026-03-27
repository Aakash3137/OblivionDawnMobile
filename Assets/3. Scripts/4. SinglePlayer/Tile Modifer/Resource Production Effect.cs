using UnityEngine;

[CreateAssetMenu(fileName = "Resource Production Effect", menuName = "Tile Effects/Resource Production Effect")]
public class ResourceProductionEffect : TileEffect
{
    [field: SerializeField] public GameObject vfxPrefab { get; private set; }

    public override void ApplyVisuals(Tile tile)
    {
        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, tile.transform.position, Quaternion.identity, tile.transform);
            vfx.transform.localPosition = Vector3.up * yOffset;
        }
    }

    public override void ApplyEffect(Tile tile)
    {

    }
}
