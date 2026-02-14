using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUi : MonoBehaviour
{
    public UnitNameEnum unitName;
   // public BuildingNameEnum buildingName;
    public bool IsUnitBuilding = false;
    
    private void Start()
    {
        Debug.Log("Buttonui start");
        gameObject.GetComponent<Button>().onClick.AddListener(() => Build(unitName));
    }

    public void Build(UnitNameEnum buildingName)
    {
        Debug.Log($"[BuildUi] Build called with name: {buildingName.ToString()}");
        if (TileSelectionManager.Instance.selectedTile == null)
        {
            Debug.LogWarning("[BuildUi] TileSelectionManager.Instance is NULL");
            return;
        }
        var selected = TileSelectionManager.Instance.selectedTile;
        if (selected == null)
        {
            Debug.LogWarning("[BuildUi] No tile selected - cannot build");
            return;
        }

        Debug.Log($"[BuildUi] Selected tile: {selected.name} (NetworkObject Id: {selected.Object?.Id.ToString() ?? "null"})");
        selected.RequestBuild(buildingName);
    }
    
   
}