using UnityEngine;
using UnityEngine.UI;

public class BuildUi : MonoBehaviour
{
    [SerializeField] private Button buildButtonPrefab;
    private void Start()
    {
        Debug.Log("Buttonui start");
        buildButtonPrefab.onClick.AddListener(() => Build(buildButtonPrefab.name));
    }

    public void Build(string buildingName)
    {
        Debug.Log($"[BuildUi] Build called with name: {buildingName}");
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
