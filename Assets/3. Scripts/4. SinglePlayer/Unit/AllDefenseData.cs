using UnityEngine;

[CreateAssetMenu(fileName = "All Data", menuName = "RTS/All Defense Data")]
public class AllDefenseData : ScriptableObject
{
    [Header("Medieval Units")]
    public BuildingData[] defenseBuildingDataMedieval;

    [Header("Present Units")]
    public BuildingData[] defenseBuildingDataPresent;

    [Header("Futuristic Units")]
    public BuildingData[] defenseBuildingDataFuturistic;

    [Header("Galvadore Units")]
    public BuildingData[] defenseBuildingDataGalvadore;
}

[System.Serializable]
public struct BuildingData
{
    public GameObject prefab;
    public SpriteRenderer icon;
}