using UnityEngine;

[CreateAssetMenu(menuName = "Multiplayer/Faction")]
public class MP_Faction : ScriptableObject
{
    [Header("Faction Info")]
    public string factionName; // Past / Present / Future / Monster

    [Header("Main Building")]
    public GameObject mainBuildingPrefab;
    public MP_BuildingStats mainBuildingStats;

    [Header("Buildings")]
    public GameObject defenceBuildingPrefab;
    public MP_BuildingStats defenceBuildingStats;

    public GameObject unitBuildingPrefab;
    public MP_BuildingStats unitBuildingStats;

    public GameObject resourceBuildingPrefab;
    public MP_BuildingStats resourceBuildingStats;

    [Header("Military Units")]
    public GameObject gunmanPrefab;
    public MP_UnitStats gunmanStats;

    public GameObject tankPrefab;
    public MP_UnitStats tankStats;

    public GameObject aircraftPrefab;
    public MP_UnitStats aircraftStats;
    
    [Header("New")]
    [Header ("UnitBuilding")]
    public GameObject AirUnitBuildingPrefab;
    public GameObject MeleeUnitBuildingPrefab;
    public GameObject RangedUnitBuildingPrefab;
    public GameObject AOERangedUnitBuildingPrefab;
    
    [Header ("DefenseBuilding")]
    public GameObject AntiAirBuildingPrefab;
    public GameObject AntiTankBuildingPrefab;
    public GameObject TurretBuildingPrefab;
    
    [Header("ResourceBuilding")]
    public GameObject FoodBuildingPrefab;
    public GameObject GoldBuildingPrefab;
    public GameObject PowerBuildingPrefab;
    public GameObject MetalBuildingPrefab;
    
}