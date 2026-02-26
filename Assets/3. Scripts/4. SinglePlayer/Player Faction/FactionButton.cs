using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [Header("Faction Info")]
    [SerializeField] private FactionName faction;        // enum value (Past, Present, Future, Monster)
    [SerializeField] private AllFactionsData data;       // reference to the ScriptableObject asset
    [SerializeField] private DecSelectionData decSelectionData;
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
        decSelectionData.CurrentFaction = faction;

        GameData.SelectedFactionName = faction.ToString();
        Debug.Log($"[FactionButton] Player faction selected: {GameData.SelectedFaction}");
        Debug.Log($"[FactionButton] Player faction name selected: {GameData.SelectedFactionName}");
        Debug.Log($"[FactionButton] Player MP faction selected: {GameData.SelectedMPFaction.factionName}");

        if (GameData.GameModeType == "Campaign")
        {
            OnFactionSelected();
        }
        else
        {
            HomeUIManager.Instance.OnFactionClicked();
            DeactivateFactionPanel();
        }

        GameStateManager.Instance.ChangeState(GameState.PLAYING);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    void OnFactionSelected()
    {
        DeactivateFactionPanel();
        HomeUIManager.Instance.SwitchPanel(HomeUIManager.Instance.HomePanel, HomeUIManager.Instance.LoadingPanel);
        StartCoroutine(LoadSceneAfterDelay(2));
    }

    private void DeactivateFactionPanel()
    {

        // if (factionPanel != null)
        // {
        //     factionPanel.alpha = 0f;
        //     factionPanel.interactable = false;
        //     factionPanel.blocksRaycasts = false;
        // }
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SinglePlayerScene");
    }
}
