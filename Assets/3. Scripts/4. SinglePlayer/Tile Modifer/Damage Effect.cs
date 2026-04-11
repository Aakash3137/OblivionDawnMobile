using UnityEngine;

[CreateAssetMenu(fileName = "Damage Effect", menuName = "Tile Effects/Damage Effect")]
public class DamageEffect : TileEffect
{
    [field: SerializeField] public GameObject vfxPrefab { get; private set; }
    [field: SerializeField] public GameObject cornerVfxPrefab { get; private set; }
    [field: SerializeField] public float buffStrength { get; private set; }
    [field: SerializeField] public float cornerBuffStrength { get; private set; }

    public override void ApplyVisuals(Tile tile)
    {
        if (tile == null || vfxPrefab == null)
            return;

        if (CubeGridManager.Instance.IsCornerTile(tile))
        {
            var vfx = Instantiate(cornerVfxPrefab, tile.transform.position, Quaternion.identity, tile.transform);
            vfx.transform.localPosition = Vector3.up * yOffset;
            tile.tileEffectPrefab = vfx;
        }
        else
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

        if (CubeGridManager.Instance.IsCornerTile(tile))
        {
            building.BuffDamageStats(cornerBuffStrength);
        }
        else
        {
            building.BuffDamageStats(buffStrength);
        }
    }
}
