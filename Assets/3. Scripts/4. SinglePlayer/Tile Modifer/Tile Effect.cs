using UnityEngine;
public abstract class TileEffect : ScriptableObject
{
    [field: SerializeField, Range(0, 10)] public int visualPriority { get; private set; }
    [field: SerializeField] public float yOffset { get; private set; }
    [field: SerializeField, Tooltip("In percentage"), Range(0, 200)] public float effectBuffStrength { get; private set; }

    public abstract void ApplyVisuals(Tile tile);
    public virtual void ApplyEffect(Tile tile) { }
}