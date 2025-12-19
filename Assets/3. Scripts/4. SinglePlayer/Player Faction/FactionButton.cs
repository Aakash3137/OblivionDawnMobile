// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class FactionButton : MonoBehaviour
// {
//     [Header("Faction Info")]
//     [SerializeField]
//     string factionName;

//     [Header("Main Building")]
//     [SerializeField]
//     internal GameObject mainBuildingPrefab;

//     [Header("Other Buildings")]
//     [SerializeField]
//     internal GameObject[] buildingPrefabs; // gold mine, wood mill, steel mine, etc.

//     [Header("UI References")]
//     [SerializeField] CanvasGroup factionPanel; // drag your PlayerFaction Panel here in Inspector
//     [SerializeField] private GameObject HomePanel, LoadingPanel;

//     private Button button;

//     private void Start()
//     {
//         button = GetComponent<Button>();
//         button.onClick.AddListener(OnFactionSelected);
//     }

//     void OnFactionSelected()
//     {
//         // Tell the spawner which faction was chosen
//         MainBuildingSpawner.Instance.SetPlayerFaction(this);

//         // Hide the faction panel
//         if (factionPanel != null)
//         {
//             factionPanel.alpha = 0f;
//             factionPanel.interactable = false;
//             factionPanel.blocksRaycasts = false;
//         }


//         SwitchPanel(HomePanel, LoadingPanel);
//         // TODO: Load Campaign Scene
//         StartCoroutine(LoadSceneAfterDelay(2));
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

//     private void Start()
//     {
//         button = GetComponent<Button>();
//         button.onClick.AddListener(OnFactionSelected);
//     }

//     void OnFactionSelected()
//     {
//         // Save faction choice globally
//         GameData.SelectedMainBuildingPrefab = mainBuildingPrefab;
//         GameData.SelectedFactionName = factionName;

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

    [Header("Other Buildings")]
    [SerializeField] internal GameObject[] buildingPrefabs;

    [Header("UI References")]
    [SerializeField] CanvasGroup factionPanel;
    [SerializeField] private GameObject HomePanel, LoadingPanel;

    private Button button;

    void Start()
    {
        // Register this faction prefab globally
        if (!GameData.AllFactionPrefabs.Contains(mainBuildingPrefab))
            GameData.AllFactionPrefabs.Add(mainBuildingPrefab);

        button = GetComponent<Button>();
        button.onClick.AddListener(OnFactionSelected);
    }
    void OnFactionSelected()
    {
        // Save faction choice globally
        GameData.SelectedMainBuildingPrefab = mainBuildingPrefab;
        GameData.SelectedFactionName = factionName;



        // Debug logs
        Debug.Log($"[FactionButton] Faction selected: {GameData.SelectedFactionName}");
        Debug.Log($"[FactionButton] Game mode selected: {GameData.GameModeType}");

        // Hide faction panel
        DeactivateFactiongPanel();

        SwitchPanel(HomePanel, LoadingPanel);

        // Load campaign scene
        StartCoroutine(LoadSceneAfterDelay(2));
    }

    private void DeactivateFactiongPanel()
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
