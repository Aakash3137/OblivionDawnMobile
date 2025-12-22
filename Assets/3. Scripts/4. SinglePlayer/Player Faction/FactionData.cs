using UnityEngine;

[CreateAssetMenu(fileName = "FactionsData", menuName = "RTS/Factions Data")]
public class FactionsData : ScriptableObject
{
    [System.Serializable]
    public class BuildingSlot
    {
        public GameObject prefab;          // shared prefab
        public Material playerMaterial;    // material for player side
        public Material enemyMaterial;     // material for enemy side
    }

    [System.Serializable]
    public class FactionBlock
    {
        [Header("Main Building")]
        public BuildingSlot mainBuilding;

        [Header("Gold Mine")]
        public BuildingSlot goldMine;

        [Header("Unit Building")]
        public BuildingSlot unitBuilding;

        [Header("Turret Building")]
        public BuildingSlot turretBuilding;
    }

    [Header("Past Faction")]
    public FactionBlock past;

    [Header("Present Faction")]
    public FactionBlock present;

    [Header("Future Faction")]
    public FactionBlock future;

    [Header("Monster Faction")]
    public FactionBlock monster;
}
