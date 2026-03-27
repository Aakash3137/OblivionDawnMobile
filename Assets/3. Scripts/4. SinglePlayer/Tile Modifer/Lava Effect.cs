using UnityEngine;

[CreateAssetMenu(fileName = "Lava Effect", menuName = "Tile Effects/Lava Effect")]
public class LavaEffect : TileEffect
{
    [field: SerializeField] public Material lavaMaterial { get; private set; }

    public override void ApplyVisuals(Tile tile)
    {
        if (tile == null)
            return;

        tile.ChangeSide(Side.Neutral);
        tile.OverrideMaterial(lavaMaterial);
        tile.transform.localPosition += Vector3.down * yOffset;
    }
}
