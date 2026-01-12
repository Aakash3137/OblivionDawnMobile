using UnityEngine;

[CreateAssetMenu(fileName = "AllFactionsData", menuName = "RTS/All Factions Data")]
public class AllFactionsData : ScriptableObject
{
    [System.Serializable]
    public class BuildingSlot
    {
        [Header("Prefab (shared for both sides)")]
        public GameObject prefab;

        [Header("Materials")]
        public Material playerMaterial;
        public Material enemyMaterial;
    }

    // -------------------------
    // Past Faction
    // -------------------------
    [Header("Past Faction")]
    public BuildingSlot pastMainBuilding;
    public BuildingSlot pastFoodBuilding;
    public BuildingSlot pastGoldBuilding;
    public BuildingSlot pastMetalBuilding;
    public BuildingSlot pastPowerBuilding;
    public BuildingSlot pastAirBuilding;
    public BuildingSlot pastInfantryBuilding;
    public BuildingSlot pastMeleeBuilding;
    public BuildingSlot pastTankBuilding;
    public BuildingSlot pastAntiAirBuilding;
    public BuildingSlot pastAntiTankBuilding;
    public BuildingSlot pastTurretBuilding;
    public BuildingSlot pastWallBuilding;

    // -------------------------
    // Present Faction
    // -------------------------
    [Header("Present Faction")]
    public BuildingSlot presentMainBuilding;
    public BuildingSlot presentFoodBuilding;
    public BuildingSlot presentGoldBuilding;
    public BuildingSlot presentMetalBuilding;
    public BuildingSlot presentPowerBuilding;
    public BuildingSlot presentAirBuilding;
    public BuildingSlot presentInfantryBuilding;
    public BuildingSlot presentTankBuilding;
    public BuildingSlot presentAntiAirBuilding;
    public BuildingSlot presentAntiTankBuilding;
    public BuildingSlot presentTurretBuilding;

    // -------------------------
    // Future Faction
    // -------------------------
    [Header("Future Faction")]
    public BuildingSlot futureMainBuilding;
    public BuildingSlot futureFoodBuilding;
    public BuildingSlot futureGoldBuilding;
    public BuildingSlot futureMetalBuilding;
    public BuildingSlot futurePowerBuilding;
    public BuildingSlot futureAirBuilding;
    public BuildingSlot futureInfantryBuilding;
    public BuildingSlot futureTankBuilding;
    public BuildingSlot futureAntiAirBuilding;
    public BuildingSlot futureAntiTankBuilding;
    public BuildingSlot futureTurretBuilding;

    // -------------------------
    // Monster Faction
    // -------------------------
    [Header("Monster Faction")]
    public BuildingSlot monsterMainBuilding;
    public BuildingSlot monsterFoodBuilding;
    public BuildingSlot monsterGoldBuilding;
    public BuildingSlot monsterMetalBuilding;
    public BuildingSlot monsterPowerBuilding;
    public BuildingSlot monsterAirBuilding;
    public BuildingSlot monsterInfantryBuilding;
    public BuildingSlot monsterTankBuilding;
    public BuildingSlot monsterAntiAirBuilding;
    public BuildingSlot monsterAntiTankBuilding;
    public BuildingSlot monsterTurretBuilding;
}
