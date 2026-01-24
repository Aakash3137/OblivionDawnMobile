using UnityEngine;

[CreateAssetMenu(fileName = "All Defense Building Data", menuName = "Data/All Defense Building Data")]
public class AllDefenseBuildingData : ScriptableObject
{
    [Header("Medieval")]
    public DefenseBuilding[] airDefenseBuildingDataMedieval;
    public DefenseBuilding[] infantryDefenseBuildingDataMedieval;
    public DefenseBuilding[] meleeDefenseBuildingDataMedieval;
    public DefenseBuilding[] tankDefenseBuildingDataMedieval;

    [Header("Present")]
    public DefenseBuilding[] airDefenseBuildingDataPresent;
    public DefenseBuilding[] infantryDefenseBuildingDataPresent;
    public DefenseBuilding[] meleeDefenseBuildingDataPresent;
    public DefenseBuilding[] tankDefenseBuildingDataPresent;

    [Header("Futuristic")]
    public DefenseBuilding[] airDefenseBuildingDataFuturistic;
    public DefenseBuilding[] infantryDefenseBuildingDataFuturistic;
    public DefenseBuilding[] meleeDefenseBuildingDataFuturistic;
    public DefenseBuilding[] tankDefenseBuildingDataFuturistic;

    [Header("Galvadore")]
    public DefenseBuilding[] airDefenseBuildingDataGalvadore;
    public DefenseBuilding[] infantryDefenseBuildingDataGalvadore;
    public DefenseBuilding[] meleeDefenseBuildingDataGalvadore;
    public DefenseBuilding[] tankDefenseBuildingDataGalvadore;
}

[System.Serializable]
public struct DefenseBuilding
{
    public GameObject prefab;
    public SpriteRenderer icon;
}