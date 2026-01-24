using UnityEngine;

[CreateAssetMenu(fileName = "All Offense Building Data", menuName = "Data/All Offense Building Data")]
public class AllOffenseBuildingData : ScriptableObject
{
    [Header("Medieval")]
    public OffenseBuilding[] airOffenseBuildingDataMedieval;
    public OffenseBuilding[] infantryOffenseBuildingDataMedieval;
    public OffenseBuilding[] meleeOffenseBuildingDataMedieval;
    public OffenseBuilding[] tankOffenseBuildingDataMedieval;

    [Header("Present")]
    public OffenseBuilding[] airOffenseBuildingDataPresent;
    public OffenseBuilding[] infantryOffenseBuildingDataPresent;
    public OffenseBuilding[] meleeOffenseBuildingDataPresent;
    public OffenseBuilding[] tankOffenseBuildingDataPresent;

    [Header("Futuristic")]
    public OffenseBuilding[] airOffenseBuildingDataFuturistic;
    public OffenseBuilding[] infantryOffenseBuildingDataFuturistic;
    public OffenseBuilding[] meleeOffenseBuildingDataFuturistic;
    public OffenseBuilding[] tankOffenseBuildingDataFuturistic;

    [Header("Galvadore")]
    public OffenseBuilding[] airOffenseBuildingDataGalvadore;
    public OffenseBuilding[] infantryOffenseBuildingDataGalvadore;
    public OffenseBuilding[] meleeOffenseBuildingDataGalvadore;
    public OffenseBuilding[] tankOffenseBuildingDataGalvadore;
}

[System.Serializable]
public struct OffenseBuilding
{
    public GameObject prefab;
    public SpriteRenderer icon;
}