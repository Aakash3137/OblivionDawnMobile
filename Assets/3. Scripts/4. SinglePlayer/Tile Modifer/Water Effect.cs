using UnityEngine;

[CreateAssetMenu(fileName = "Water Effect", menuName = "Tile Effects/Water Effect")]
public class WaterEffect : TileEffect
{
    [field: SerializeField] public Material waterMaterial { get; private set; }

    public override void ApplyVisuals(Tile tile)
    {
        if (tile == null)
            return;

        tile.ChangeSide(Side.Neutral);
        CubeGridManager.Instance.TileOccupied(Side.Neutral, tile);
        tile.OverrideMaterial(waterMaterial);
        tile.transform.localPosition += Vector3.down * yOffset;
    }
}
