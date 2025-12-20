using UnityEngine;

[CreateAssetMenu(fileName = "FactionData", menuName = "RTS/Faction Data")]
public class FactionData : ScriptableObject
{
    [Header("Faction Info")]
    public string factionName;

    [Header("Main Building")]
    public GameObject mainBuildingPrefab;

    [Header("Player Building Prefabs")]
    public GameObject[] playerBuildingPrefabs;

    [Header("Enemy Building Prefabs")]
    public GameObject[] enemyBuildingPrefabs;
}
