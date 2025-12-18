using UnityEngine;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    [SerializeField]
    string factionName;

    [Header("Main Building")]
    [SerializeField]
    internal GameObject mainBuildingPrefab;

    [Header("Other Buildings")]
    [SerializeField]
    internal GameObject[] buildingPrefabs; // gold mine, wood mill, steel mine, etc.

    [Header("UI References")]
    [SerializeField] CanvasGroup factionPanel; // drag your PlayerFaction Panel here in Inspector

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

        // Hide the faction panel
        if (factionPanel != null)
        {
            factionPanel.alpha = 0f;
            factionPanel.interactable = false;
            factionPanel.blocksRaycasts = false;
        }
    }
}
