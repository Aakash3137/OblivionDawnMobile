using UnityEngine;

[CreateAssetMenu(fileName = "All Data", menuName = "RTS/All Units Data")]
public class AllUnitData : ScriptableObject
{
    [Header("Medieval Units")]
    public UnitData[] airUnitDataMedieval;
    public UnitData[] infantryUnitDataMedieval;
    public UnitData[] meleeUnitDataMedieval;
    public UnitData[] tankUnitDataMedieval;

    [Header("Present Units")]
    public UnitData[] airUnitDataPresent;
    public UnitData[] infantryUnitDataPresent;
    public UnitData[] meleeUnitDataPresent;
    public UnitData[] tankUnitDataPresent;

    [Header("Futuristic Units")]
    public UnitData[] airUnitDataFuturistic;
    public UnitData[] infantryUnitDataFuturistic;
    public UnitData[] meleeUnitDataFuturistic;
    public UnitData[] tankUnitDataFuturistic;

    [Header("Galvadore Units")]
    public UnitData[] airUnitDataGalvadore;
    public UnitData[] infantryUnitDataGalvadore;
    public UnitData[] meleeUnitDataGalvadore;
    public UnitData[] tankUnitDataGalvadore;
}

[System.Serializable]
public struct UnitData
{
    public UnitProduceStatsSO unitData;
    public SpriteRenderer icon;
}