using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    [SerializeField] private FactionName faction;        // enum value (Past, Present, Future, Monster)
    [SerializeField] private AllFactionsData data;       // reference to the ScriptableObject asset
    [SerializeField] MP_Faction mpFaction;

    [SerializeField] private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckGameMode);
    }

    public void CheckGameMode()
    {
        GameData.SelectedMPFaction = mpFaction;
        // Store player’s choice globally
        GameData.SelectedFaction = faction;   // <-- critical line
        GameData.AllFactionsData = data;

        GameData.SelectedFactionName = faction.ToString();
        Debug.Log($"[FactionButton] Player faction selected: {GameData.SelectedFaction}");
        Debug.Log($"[FactionButton] Player faction name selected: {GameData.SelectedFactionName}");
        Debug.Log($"[FactionButton] Player MP faction selected: {GameData.SelectedMPFaction.factionName}");

        // HomeUIManager.Instance.ShowPanel(PanelName.Loading);
        HomeUIManager.Instance.LoadingPanel.SetActive(true);
        if (GameData.GameModeType == "Campaign")
        {
            StartCoroutine(LoadSceneAfterDelay(2, "SinglePlayerScene"));
        }
        else
        {
            StartCoroutine(LoadSceneAfterDelay(2, "GameScene"));
        }
    }

    
    private IEnumerator LoadSceneAfterDelay(float delay, string sceneName )
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
