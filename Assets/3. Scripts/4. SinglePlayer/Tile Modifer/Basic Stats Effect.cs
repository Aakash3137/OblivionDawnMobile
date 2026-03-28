using UnityEngine;

[CreateAssetMenu(fileName = "Basic Stats Effect", menuName = "Tile Effects/Basic Stats Effect")]
public class BasicStatsEffect : TileEffect
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
        if (!tile.hasBuilding)
            return;

        var building = tile.currentBuilding;
    }
}
