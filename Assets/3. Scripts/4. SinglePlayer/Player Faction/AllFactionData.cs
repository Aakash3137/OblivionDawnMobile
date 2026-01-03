using UnityEngine;

[CreateAssetMenu(fileName = "AllFactionsData", menuName = "RTS/All Factions Data")]
public class AllFactionsData : ScriptableObject
{
    public Material playerResourceMaterial;
    public Material enemyResourceMaterial;
    public Material playerUnitMaterial;
    public Material enemyUnitMaterial;
    public Material turretMat;

    void OnValidate()
    {
        if (playerResourceMaterial == null || enemyResourceMaterial == null || playerUnitMaterial == null || enemyUnitMaterial == null) return;
        
        pastFoodBuilding.playerMaterial = playerResourceMaterial;
        pastFoodBuilding.enemyMaterial = enemyResourceMaterial;
        pastGoldBuilding.playerMaterial = playerResourceMaterial;
        pastGoldBuilding.enemyMaterial = enemyResourceMaterial;
        pastMetalBuilding.playerMaterial = playerResourceMaterial;
        pastMetalBuilding.enemyMaterial = enemyResourceMaterial;
        pastPowerBuilding.playerMaterial = playerResourceMaterial;
        pastPowerBuilding.enemyMaterial = enemyResourceMaterial;
        presentFoodBuilding.playerMaterial = playerResourceMaterial;
        presentFoodBuilding.enemyMaterial = enemyResourceMaterial;
        presentGoldBuilding.playerMaterial = playerResourceMaterial;
        presentGoldBuilding.enemyMaterial = enemyResourceMaterial;
        presentMetalBuilding.playerMaterial = playerResourceMaterial;
        presentMetalBuilding.enemyMaterial = enemyResourceMaterial;
        presentPowerBuilding.playerMaterial = playerResourceMaterial;
        presentPowerBuilding.enemyMaterial = enemyResourceMaterial;
        futureFoodBuilding.playerMaterial = playerResourceMaterial;
        futureFoodBuilding.enemyMaterial = enemyResourceMaterial;
        futureGoldBuilding.playerMaterial = playerResourceMaterial;
        futureGoldBuilding.enemyMaterial = enemyResourceMaterial;
        futureMetalBuilding.playerMaterial = playerResourceMaterial;
        futureMetalBuilding.enemyMaterial = enemyResourceMaterial;
        futurePowerBuilding.playerMaterial = playerResourceMaterial;
        futurePowerBuilding.enemyMaterial = enemyResourceMaterial;
        monsterFoodBuilding.playerMaterial = playerResourceMaterial;
        monsterFoodBuilding.enemyMaterial = enemyResourceMaterial;
        monsterGoldBuilding.playerMaterial = playerResourceMaterial;
        monsterGoldBuilding.enemyMaterial = enemyResourceMaterial;
        monsterMetalBuilding.playerMaterial = playerResourceMaterial;
        monsterMetalBuilding.enemyMaterial = enemyResourceMaterial;
        monsterPowerBuilding.playerMaterial = playerResourceMaterial;
        monsterPowerBuilding.enemyMaterial = enemyResourceMaterial;

        pastInfantryBuilding.playerMaterial = playerUnitMaterial;
        pastInfantryBuilding.enemyMaterial = enemyUnitMaterial;
        presentInfantryBuilding.playerMaterial = playerUnitMaterial;
        presentInfantryBuilding.enemyMaterial = enemyUnitMaterial;
        futureInfantryBuilding.playerMaterial = playerUnitMaterial;
        futureInfantryBuilding.enemyMaterial = enemyUnitMaterial;
        monsterInfantryBuilding.playerMaterial = playerUnitMaterial;
        monsterInfantryBuilding.enemyMaterial = enemyUnitMaterial;

        pastTurretBuilding.playerMaterial = turretMat;
        pastTurretBuilding.enemyMaterial = turretMat;
        presentTurretBuilding.playerMaterial = turretMat;
        presentTurretBuilding.enemyMaterial = turretMat;
        futureTurretBuilding.playerMaterial = turretMat;
        futureTurretBuilding.enemyMaterial = turretMat;
        monsterTurretBuilding.playerMaterial = turretMat;
        monsterTurretBuilding.enemyMaterial = turretMat;
    }

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
    public BuildingSlot pastTankBuilding;
    public BuildingSlot pastAntiAirBuilding;
    public BuildingSlot pastAntiTankBuilding;
    public BuildingSlot pastTurretBuilding;

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
