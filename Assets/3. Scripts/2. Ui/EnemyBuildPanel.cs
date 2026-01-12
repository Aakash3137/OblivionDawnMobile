using UnityEngine;
using UnityEngine.UI;

public class EnemyBuildPanel : MonoBehaviour
{
    private Tile currentTile;

    [SerializeField] private Button enemyAirBuilding;
    [SerializeField] private Button enemyInfantryBuilding;
    [SerializeField] private Button enemyTankBuilding;
    [SerializeField] private AllFactionsData factionData;

    private void Start()
    {
        enemyAirBuilding.onClick.AddListener(PlaceEnemyAirBuilding);
        enemyInfantryBuilding.onClick.AddListener(PlaceEnemyInfantryBuilding);
        enemyTankBuilding.onClick.AddListener(PlaceEnemyTankBuilding);
        gameObject.SetActive(false);
    }
    public void PlaceEnemyAirBuilding()
    {
        var slot = factionData.pastAirBuilding;
        PlaceBuilding(slot);
    }
    public void PlaceEnemyInfantryBuilding()
    {
        var slot = factionData.pastInfantryBuilding;
        PlaceBuilding(slot);
    }
    public void PlaceEnemyTankBuilding()
    {
        var slot = factionData.pastTankBuilding;
        PlaceBuilding(slot);
    }

    private void PlaceBuilding(AllFactionsData.BuildingSlot slot)
    {
        if (currentTile == null || slot == null || slot.prefab == null) return;

        if (currentTile.hasBuilding) return;

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;

        Instantiate(slot.prefab, spawnPos, Quaternion.identity, currentTile.transform);

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
