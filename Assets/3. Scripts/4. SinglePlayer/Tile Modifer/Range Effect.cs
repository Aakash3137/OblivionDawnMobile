using UnityEngine;

[CreateAssetMenu(fileName = "Range Effect", menuName = "Tile Effects/Range Effect")]
public class RangeEffect : TileEffect
{
    [field: SerializeField] public GameObject vfxPrefab { get; private set; }
    [field: SerializeField] public GameObject cornerVfxPrefab { get; private set; }
    [field: SerializeField] public float buffStrength { get; private set; }
    [field: SerializeField] public float cornerBuffStrength { get; private set; }

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

        var building = tile.currentBuilding as DefenseBuildingStats;

        if (building == null)
            return;
    }
}
