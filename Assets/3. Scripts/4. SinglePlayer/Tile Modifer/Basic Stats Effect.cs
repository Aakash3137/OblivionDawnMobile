using UnityEngine;

[CreateAssetMenu(fileName = "Basic Stats Effect", menuName = "Tile Effects/Basic Stats Effect")]
public class BasicStatsEffect : TileEffect
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

        var building = tile.currentBuilding;

        if (building == null)
            return;

        if (CubeGridManager.Instance.IsCornerTile(tile))
        {
            building.BuffBasicStats(cornerBuffStrength);
        }
        else
        {
            building.BuffBasicStats(buffStrength);
        }
    }

}
