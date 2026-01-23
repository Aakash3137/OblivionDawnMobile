using UnityEngine;
using UnityEngine.UI;

public class EnemyBuildPanel : MonoBehaviour
{
    private Tile currentTile;

    [SerializeField] private Button enemyAirBuilding;
    [SerializeField] private Button enemyInfantryBuilding;
    [SerializeField] private Button enemyTankBuilding;
    [SerializeField] private AllFactionsData factionData;

    [SerializeField] private FactionName EnemyfactionName;
    
    private void Awake()
    {
        MainBuildingSpawner.SetFactionNameThroughEnemyBuildPanel(EnemyfactionName);
    }
    private void Start()
    {
        enemyAirBuilding.onClick.AddListener(PlaceEnemyAirBuilding);
        enemyInfantryBuilding.onClick.AddListener(PlaceEnemyInfantryBuilding);
        enemyTankBuilding.onClick.AddListener(PlaceEnemyTankBuilding);
        gameObject.SetActive(false);
    }
    public void PlaceEnemyAirBuilding()
    {
        var slot = GetBuildingByType("Air");  
        PlaceBuilding(slot);
    }
    public void PlaceEnemyInfantryBuilding()
    {
        var slot = GetBuildingByType("Infantry");
        PlaceBuilding(slot);
    }
    public void PlaceEnemyTankBuilding()
    {
        var slot = GetBuildingByType("Tank");
        PlaceBuilding(slot);
    }

    // updated code with enemy faction selection from inspector.
    private GameObject GetBuildingByType(string buildingType)
    {
        switch (EnemyfactionName)
        {
            case FactionName.Medieval:
                return buildingType switch
                {
                    "Air" => factionData.medievalAirBuilding,
                    "Infantry" => factionData.medievalInfantryBuilding,
                    "Tank" => factionData.medievalTankBuilding,
                    _ => null
                };
            case FactionName.Present:
                return buildingType switch
                {
                    "Air" => factionData.presentAirBuilding,
                    "Infantry" => factionData.presentInfantryBuilding,
                    "Tank" => factionData.presentTankBuilding,
                    _ => null
                };
            case FactionName.Futuristic:
                return buildingType switch
                {
                    "Air" => factionData.futureAirBuilding,
                    "Infantry" => factionData.futureInfantryBuilding,
                    "Tank" => factionData.futureTankBuilding,
                    _ => null
                };
            case FactionName.Galvadore:
                return buildingType switch
                {
                    "Air" => factionData.galvadoreAirBuilding,
                    "Infantry" => factionData.galvadoreInfantryBuilding,
                    "Tank" => factionData.galvadoreTankBuilding,
                    _ => null
                };
            default:
                return null;
        }
    }

    private void PlaceBuilding(GameObject buildingPrefab)
    {
        if (currentTile == null || buildingPrefab == null || buildingPrefab == null) return;

        if (currentTile.hasBuilding) return;

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;

        Instantiate(buildingPrefab, spawnPos, Quaternion.identity, currentTile.transform);

        currentTile.SetBuildingPlaced();

        CloseBuildPanel();
    }
    public void CloseBuildPanel()
    {
        currentTile = null;
        gameObject.SetActive(false);
    }
    public void OpenBuildPanel(Tile tile)
    {
        currentTile = tile;
        gameObject.SetActive(true);
    }
}
