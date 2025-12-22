// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class FactionButton : MonoBehaviour
// {
//     [Header("Faction Info")]
//     [SerializeField] string factionName;

//     [SerializeField] MP_Faction mpFaction; //=========

//     [Header("Main Building")]
//     [SerializeField] internal GameObject mainBuildingPrefab;

//     [Header("Player Buildings for this faction")]
//     [SerializeField] internal GameObject[] buildingPrefabs;

//     [Header("Enemy Buildings for this faction (optional)")]
//     [SerializeField] internal GameObject[] enemyBuildingPrefabs;

//     [Header("UI References")]
//     [SerializeField] CanvasGroup factionPanel;

//     private Button button;

//     void Start()
//     {
//         button = GetComponent<Button>();
//         //button.onClick.AddListener(OnFactionSelected);
//         button.onClick.AddListener(CheckGameMode);
//     }

//     void CheckGameMode()
//     {
//         GameData.SelectedFactionName = factionName; //======
//         GameData.SelectedMPFaction = mpFaction; //=========

//         if (GameData.GameModeType == "Campaign")
//         {
//             OnFactionSelected();
//         }
//         else
//         {
//             HomeUIManager.Instance.OnFactionClicked();
//             DeactivateFactionPanel();
//         }
//     }

//     // void OnFactionSelected()
//     // {
//     //     if (GameData.PrefabStore == null)
//     //     {
//     //         Debug.LogError("[FactionButton] PrefabStore not assigned.");
//     //         return;
//     //     }

//     //     // Inject all prefabs into the store
//     //     GameData.PrefabStore.SetFromFaction(buildingPrefabs, enemyBuildingPrefabs);

//     //     // Also set the main building prefab
//     //     GameData.PrefabStore.playerMainBuildingPrefab = mainBuildingPrefab;

//     //     GameData.SelectedFactionName = factionName;

//     //     Debug.Log($"[FactionButton] Faction selected: {factionName}, main building injected: {mainBuildingPrefab.name}");

//     //     DeactivateFactionPanel();
//     //     HomeUIManager.Instance.SwitchPanel(HomeUIManager.Instance.HomePanel, HomeUIManager.Instance.LoadingPanel);
//     //     StartCoroutine(LoadSceneAfterDelay(2));
//     // }




//     void OnFactionSelected()
//     {
//         if (GameData.PrefabStore == null)
//         {
//             Debug.LogError("[FactionButton] PrefabStore not assigned.");
//             return;
//         }

//         // Add this faction’s prefabs into the global store
//         GameData.PrefabStore.AddFaction(mainBuildingPrefab, buildingPrefabs, enemyBuildingPrefabs);

//         // Mark player’s choice
//         GameData.PrefabStore.playerMainBuildingPrefab = mainBuildingPrefab;
//         GameData.SelectedFactionName = factionName;

//         Debug.Log($"[FactionButton] Player faction selected: {factionName}");

//         DeactivateFactionPanel();
//         HomeUIManager.Instance.SwitchPanel(HomeUIManager.Instance.HomePanel, HomeUIManager.Instance.LoadingPanel);
//         StartCoroutine(LoadSceneAfterDelay(2));
//     }


//     private void DeactivateFactionPanel()
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
// }



using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    [SerializeField] string factionName;
    [SerializeField] private FactionName faction;   // use enum instead of string
    [SerializeField] private AllFactionsData data;  // reference to the ScriptableObject asset

    // [SerializeField] MP_Faction mpFaction; //=========

    [Header("UI References")]
    [SerializeField] private CanvasGroup factionPanel;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckGameMode);
    }

    void CheckGameMode()
    {

        // GameData.SelectedFactionName = factionName; //====== 
        // GameData.SelectedMPFaction = mpFaction; //=========

        // Store player’s choice globally
        // GameData.SelectedFaction = faction;
        GameData.AllFactionsData = data;

        if (GameData.GameModeType == "Campaign")
        {
            OnFactionSelected();
        }
        else
        {
            HomeUIManager.Instance.OnFactionClicked();
            DeactivateFactionPanel();
        }
    }

    void OnFactionSelected()
    {
        // Debug.Log($"[FactionButton] Player faction selected: {faction}");

        DeactivateFactionPanel();
        HomeUIManager.Instance.SwitchPanel(HomeUIManager.Instance.HomePanel, HomeUIManager.Instance.LoadingPanel);
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
}
