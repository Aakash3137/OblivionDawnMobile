// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class FactionButton : MonoBehaviour
// {
//     [Header("Faction Info")]
//     [SerializeField] string factionName;

//     [Header("Main Building")]
//     [SerializeField] internal GameObject mainBuildingPrefab;

//     [Header("Other Buildings")]
//     [SerializeField] internal GameObject[] buildingPrefabs;

//     [Header("UI References")]
//     [SerializeField] CanvasGroup factionPanel;
//     [SerializeField] private GameObject HomePanel, LoadingPanel;

//     private Button button;

//     void Start()
//     {
//         // Register this faction prefab globally
//         if (!GameData.AllFactionPrefabs.Contains(mainBuildingPrefab))
//             GameData.AllFactionPrefabs.Add(mainBuildingPrefab);

//         button = GetComponent<Button>();
//         button.onClick.AddListener(OnFactionSelected);
//     }
//     void OnFactionSelected()
//     {
//         // Save faction choice globally
//         GameData.SelectedMainBuildingPrefab = mainBuildingPrefab;
//         GameData.SelectedFactionName = factionName;



//         // Debug logs
//         Debug.Log($"[FactionButton] Faction selected: {GameData.SelectedFactionName}");
//         Debug.Log($"[FactionButton] Game mode selected: {GameData.GameModeType}");

//         // Hide faction panel
//         DeactivateFactiongPanel();

//         SwitchPanel(HomePanel, LoadingPanel);

//         // Load campaign scene
//         StartCoroutine(LoadSceneAfterDelay(2));
//     }

//     private void DeactivateFactiongPanel()
//     {
//         if (factionPanel != null)
//         {
//             factionPanel.alpha = 0f;
//             factionPanel.interactable = false;
//             factionPanel.blocksRaycasts = false;
//         }
//     }

//     private IEnumerator LoadSceneAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         SceneManager.LoadScene("SinglePlayerScene");
//     }

//     private void SwitchPanel(GameObject fromPanel, GameObject toPanel)
//     {
//         if (fromPanel != null) fromPanel.SetActive(false);
//         if (toPanel != null) toPanel.SetActive(true);
//     }
// }






using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    [SerializeField] string factionName;

    [Header("Main Building")]
    [SerializeField] internal GameObject mainBuildingPrefab;

    [Header("Player Buildings for this faction")]
    [SerializeField] internal GameObject[] buildingPrefabs;

    [Header("Enemy Buildings for this faction (optional)")]
    [SerializeField] internal GameObject[] enemyBuildingPrefabs;

    [Header("UI References")]
    [SerializeField] CanvasGroup factionPanel;
    [SerializeField] private GameObject HomePanel, LoadingPanel;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnFactionSelected);
    }

    void OnFactionSelected()
    {
        if (GameData.PrefabStore == null)
        {
            Debug.LogError("[FactionButton] PrefabStore not assigned.");
            return;
        }

        // Inject all prefabs into the store
        GameData.PrefabStore.SetFromFaction(buildingPrefabs, enemyBuildingPrefabs);

        // Also set the main building prefab
        GameData.PrefabStore.playerMainBuildingPrefab = mainBuildingPrefab;

        GameData.SelectedFactionName = factionName;

        Debug.Log($"[FactionButton] Faction selected: {factionName}, main building injected: {mainBuildingPrefab.name}");

        DeactivateFactionPanel();
        SwitchPanel(HomePanel, LoadingPanel);
        StartCoroutine(LoadSceneAfterDelay(2));
    }


    private void DeactivateFactionPanel()
    {
        if (factionPanel != null)
        {
            factionPanel.alpha = 0f;
            factionPanel.interactable = false;
            factionPanel.blocksRaycasts = false;
        }
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SinglePlayerScene");
    }

    private void SwitchPanel(GameObject fromPanel, GameObject toPanel)
    {
        if (fromPanel != null) fromPanel.SetActive(false);
        if (toPanel != null) toPanel.SetActive(true);
    }
}
