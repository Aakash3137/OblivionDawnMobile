using UnityEngine;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    public string factionName;

    [Header("Main Building")]
    public GameObject mainBuildingPrefab;

    [Header("Other Buildings")]
    public GameObject[] buildingPrefabs; // gold mine, wood mill, steel mine, etc.

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnFactionSelected);
    }

    void OnFactionSelected()
    {
        // Tell the spawner which faction was chosen
        MainBuildingSpawner.Instance.SetPlayerFaction(this);
    }
}
