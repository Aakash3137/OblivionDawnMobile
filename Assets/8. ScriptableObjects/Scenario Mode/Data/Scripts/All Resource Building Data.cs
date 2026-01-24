using UnityEngine;

[CreateAssetMenu(fileName = "All Resource Building Data", menuName = "Data/All Resource Building Data")]
public class AllResourceBuildingData : ScriptableObject
{
    [Header("Medieval")]
    public ResourceBuilding[] airResourceBuildingDataMedieval;
    public ResourceBuilding[] infantryResourceBuildingDataMedieval;
    public ResourceBuilding[] meleeResourceBuildingDataMedieval;
    public ResourceBuilding[] tankResourceBuildingDataMedieval;

    [Header("Present")]
    public ResourceBuilding[] airResourceBuildingDataPresent;
    public ResourceBuilding[] infantryResourceBuildingDataPresent;
    public ResourceBuilding[] meleeResourceBuildingDataPresent;
    public ResourceBuilding[] tankResourceBuildingDataPresent;

    [Header("Futuristic")]
    public ResourceBuilding[] airResourceBuildingDataFuturistic;
    public ResourceBuilding[] infantryResourceBuildingDataFuturistic;
    public ResourceBuilding[] meleeResourceBuildingDataFuturistic;
    public ResourceBuilding[] tankResourceBuildingDataFuturistic;

    [Header("Galvadore")]
    public ResourceBuilding[] airResourceBuildingDataGalvadore;
    public ResourceBuilding[] infantryResourceBuildingDataGalvadore;
    public ResourceBuilding[] meleeResourceBuildingDataGalvadore;
    public ResourceBuilding[] tankResourceBuildingDataGalvadore;
}

[System.Serializable]
public struct ResourceBuilding
{
    public GameObject prefab;
    public SpriteRenderer icon;
}