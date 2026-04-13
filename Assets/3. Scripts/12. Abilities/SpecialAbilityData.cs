using UnityEngine;

public enum SpecialAbilityType
{
    None,
    Lightning,
    Bomb,
    Fire
}

[System.Serializable]
public class SpecialAbilityData
{
    public SpecialAbilityType type;

    [Header("Visual")]
    public GameObject vfxPrefab;
    public Sprite targetSprite;

    [Header("Damage")]
    public float damage = 1000f;
    public float damageArea = 10f;
}