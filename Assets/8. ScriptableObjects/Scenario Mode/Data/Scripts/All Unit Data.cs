using UnityEngine;

[CreateAssetMenu(fileName = "All Unit Data", menuName = "Data/All Unit Data")]
public class AllUnitData : ScriptableObject
{
    [Header("Medieval")]
    public Unit[] airUnitDataMedieval;
    public Unit[] infantryUnitDataMedieval;
    public Unit[] meleeUnitDataMedieval;
    public Unit[] tankUnitDataMedieval;

    [Header("Present")]
    public Unit[] airUnitDataPresent;
    public Unit[] infantryUnitDataPresent;
    public Unit[] meleeUnitDataPresent;
    public Unit[] tankUnitDataPresent;

    [Header("Futuristic")]
    public Unit[] airUnitDataFuturistic;
    public Unit[] infantryUnitDataFuturistic;
    public Unit[] meleeUnitDataFuturistic;
    public Unit[] tankUnitDataFuturistic;

    [Header("Galvadore")]
    public Unit[] airUnitDataGalvadore;
    public Unit[] infantryUnitDataGalvadore;
    public Unit[] meleeUnitDataGalvadore;
    public Unit[] tankUnitDataGalvadore;
}

[System.Serializable]
public struct Unit
{
    public GameObject prefab;
    public SpriteRenderer icon;
}