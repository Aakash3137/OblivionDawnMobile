using UnityEngine;

[CreateAssetMenu(fileName = "Range Effect", menuName = "Tile Effects/Range Effect")]
public class RangeEffect : TileEffect
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
