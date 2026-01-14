using UnityEngine;

[CreateAssetMenu(fileName = "AllFactionsData", menuName = "RTS/All Factions Data")]
public class AllFactionsData : ScriptableObject
{
    // -------------------------
    // Past Faction
    // -------------------------
    [Header("Medieval Faction")]
    public GameObject medievalMainBuilding;
    public GameObject medievalFoodBuilding;
    public GameObject medievalGoldBuilding;
    public GameObject medievalMetalBuilding;
    public GameObject medievalPowerBuilding;
    public GameObject medievalAirBuilding;
    public GameObject medievalInfantryBuilding;
    public GameObject medievalMeleeBuilding;
    public GameObject medievalTankBuilding;
    public GameObject medievalAntiAirBuilding;
    public GameObject medievalAntiTankBuilding;
    public GameObject pastTurretBuilding;
    public GameObject medievalWallBuilding;

    // -------------------------
    // Present Faction
    // -------------------------
    [Header("Present Faction")]
    public GameObject presentMainBuilding;
    public GameObject presentFoodBuilding;
    public GameObject presentGoldBuilding;
    public GameObject presentMetalBuilding;
    public GameObject presentPowerBuilding;
    public GameObject presentAirBuilding;
    public GameObject presentInfantryBuilding;
    public GameObject presentMeleeBuilding;
    public GameObject presentTankBuilding;
    public GameObject presentAntiAirBuilding;
    public GameObject presentAntiTankBuilding;
    public GameObject presentTurretBuilding;
    public GameObject presentWallBuilding;

    // -------------------------
    // Future Faction
    // -------------------------
    [Header("Future Faction")]
    public GameObject futureMainBuilding;
    public GameObject futureFoodBuilding;
    public GameObject futureGoldBuilding;
    public GameObject futureMetalBuilding;
    public GameObject futurePowerBuilding;
    public GameObject futureAirBuilding;
    public GameObject futureInfantryBuilding;
    public GameObject futureMeleeBuilding;
    public GameObject futureTankBuilding;
    public GameObject futureAntiAirBuilding;
    public GameObject futureAntiTankBuilding;
    public GameObject futureTurretBuilding;
    public GameObject futureWallBuilding;

    // -------------------------
    // Galvadore Faction
    // -------------------------
    [Header("Monster Faction")]
    public GameObject galvadoreMainBuilding;
    public GameObject galvadoreFoodBuilding;
    public GameObject galvadoreGoldBuilding;
    public GameObject galvadoreMetalBuilding;
    public GameObject galvadorePowerBuilding;
    public GameObject galvadoreAirBuilding;
    public GameObject galvadoreInfantryBuilding;
    public GameObject galvadoreTankBuilding;
    public GameObject galvadoreMeleeBuilding;
    public GameObject galvadoreAntiAirBuilding;
    public GameObject galvadoreAntiTankBuilding;
    public GameObject galvadoreTurretBuilding;
    public GameObject galvadoreWallBuilding;
}
