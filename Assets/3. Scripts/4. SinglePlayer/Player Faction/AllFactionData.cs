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
    public BuildingSlot pastGoldMine;
    public BuildingSlot pastUnitBuilding;
    public BuildingSlot pastTurretBuilding;

    // -------------------------
    // Present Faction
    // -------------------------
    [Header("Present Faction")]
    public BuildingSlot presentMainBuilding;
    public BuildingSlot presentGoldMine;
    public BuildingSlot presentUnitBuilding;
    public BuildingSlot presentTurretBuilding;

    // -------------------------
    // Future Faction
    // -------------------------
    [Header("Future Faction")]
    public BuildingSlot futureMainBuilding;
    public BuildingSlot futureGoldMine;
    public BuildingSlot futureUnitBuilding;
    public BuildingSlot futureTurretBuilding;

    // -------------------------
    // Monster Faction
    // -------------------------
    [Header("Monster Faction")]
    public BuildingSlot monsterMainBuilding;
    public BuildingSlot monsterGoldMine;
    public BuildingSlot monsterUnitBuilding;
    public BuildingSlot monsterTurretBuilding;
}
