using UnityEngine;

[CreateAssetMenu(fileName = "AllFactionsData", menuName = "RTS/All Factions Data")]
public class AllFactionsData : ScriptableObject
{
    // -------------------------
    // Future Faction
    // -------------------------
    [Header("Future Faction")]
    public GameObject futureMainBuilding;

    public GameObject futureAntiAirBuilding;
    public GameObject futureAntiTankBuilding;
    public GameObject futureTurretBuilding;
    public GameObject futureWallBuilding;


    public GameObject futureAirBuilding;
    public GameObject futureAOERangedBuilding;
    public GameObject futureMeleeBuilding;
    public GameObject futureRangedBuilding;

    public GameObject futureFoodBuilding;
    public GameObject futureGoldBuilding;
    public GameObject futureMetalBuilding;
    public GameObject futurePowerBuilding;



    // -------------------------
    // Galvadore Faction
    // -------------------------
    [Header("Monster Faction")]
    public GameObject galvadoreMainBuilding;
    public GameObject galvadoreAntiAirBuilding;
    public GameObject galvadoreAntiTankBuilding;
    public GameObject galvadoreTurretBuilding;
    public GameObject galvadoreWallBuilding;

    public GameObject galvadoreAirBuilding;
    public GameObject galvadoreAOERangedBuilding;
    public GameObject galvadoreMeleeBuilding;
    public GameObject galvadoreRangedBuilding;

    public GameObject galvadoreFoodBuilding;
    public GameObject galvadoreGoldBuilding;
    public GameObject galvadoreMetalBuilding;
    public GameObject galvadorePowerBuilding;


    // -------------------------
    // Past Faction
    // -------------------------
    [Header("Medieval Faction")]
    public GameObject medievalMainBuilding;
    public GameObject medievalAntiAirBuilding;
    public GameObject medievalAntiTankBuilding;
    public GameObject pastTurretBuilding;
    public GameObject medievalWallBuilding;

    public GameObject medievalAirBuilding;
    public GameObject medievalAOERangedBuilding;
    public GameObject medievalMeleeBuilding;
    public GameObject medievalRangedBuilding;

    public GameObject medievalFoodBuilding;
    public GameObject medievalGoldBuilding;
    public GameObject medievalMetalBuilding;
    public GameObject medievalPowerBuilding;

    // -------------------------
    // Present Faction
    // -------------------------
    [Header("Present Faction")]
    public GameObject presentMainBuilding;
    public GameObject presentAntiAirBuilding;
    public GameObject presentAntiTankBuilding;
    public GameObject presentTurretBuilding;
    public GameObject presentWallBuilding;

    public GameObject presentAirBuilding;
    public GameObject presentAOERangedBuilding;
    public GameObject presentMeleeBuilding;
    public GameObject presentRangedBuilding;

    public GameObject presentFoodBuilding;
    public GameObject presentGoldBuilding;
    public GameObject presentMetalBuilding;
    public GameObject presentPowerBuilding;
}
