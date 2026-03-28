using UnityEngine;

[CreateAssetMenu(fileName = "Damage Effect", menuName = "Tile Effects/Damage Effect")]
public class DamageEffect : TileEffect
{
    [field: SerializeField] public GameObject vfxPrefab { get; private set; }

    public override void ApplyVisuals(Tile tile)
    {
        if (tile == null)
            return;

        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, tile.transform.position, Quaternion.identity, tile.transform);
            vfx.transform.localPosition = Vector3.up * yOffset;
            tile.tileEffectPrefab = vfx;
        }
    }

    public override void ApplyEffect(Tile tile)
    {
        if (tile == null || !tile.hasBuilding)
            return;

        var building = tile.currentBuilding;
    }
}
