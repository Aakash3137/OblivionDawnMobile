using UnityEngine;
public abstract class TileEffect : ScriptableObject
{
    [field: SerializeField, Range(0, 10)] public int visualPriority { get; private set; }
    [field: SerializeField] public float yOffset { get; private set; }
    [field: SerializeField, Tooltip("In percentage"), Range(0, 200)] public float effectBuffStrength { get; private set; }

    public abstract void ApplyVisuals(Tile tile);
    public virtual void ApplyEffect(Tile tile) { }
    public virtual void EnableVisuals(Tile tile)
    {
        if (tile == null)
            return;

        tile.tileEffectPrefab.SetActive(true);
    }
    public virtual void DisableVisuals(Tile tile)
    {
        if (tile == null)
            return;

        tile.tileEffectPrefab.SetActive(false);
    }

    public static int CustomFloatToInt(float value)
    {
        int integerPart = (int)value;
        float fractionalPart = Mathf.Abs(value - integerPart);

        if (fractionalPart > 0.3f)
        {
            return value > 0 ? integerPart + 1 : integerPart - 1;
        }

        return integerPart;
    }
}