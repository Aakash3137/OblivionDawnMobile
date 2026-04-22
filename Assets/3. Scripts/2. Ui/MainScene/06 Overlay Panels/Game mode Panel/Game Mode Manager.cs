using UnityEngine;
using UnityEngine.UI;

public enum Mode
{
    Death_Solo,
    MultiPlayer_Type,
    PVP_Type,
    Scenario_Type,
    None
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }
    private CanvasGroup canvasGroup;
    public Mode currentMode = Mode.Death_Solo;

    [SerializeField] private DeathMatchSoloHandler deathMatchSoloHandler;
    // [SerializeField] private DeathMatchMultiHandler deathMatchMultiHandler;
    // [SerializeField] private DeathMatchLobbyHandler deathMatchLobbyHandler;
    // [SerializeField] private ScenarioSoloHandler ScenarioSoloHandler;

    [SerializeField] private Button selectFaction;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HidePanel();

        selectFaction.onClick.AddListener(OnClickSelectFaction);
        closeButton.onClick.AddListener(OnClickClosePanel);
    }

    private void OnClickSelectFaction()
    {
        GameData.gameMode = currentMode;

        Debug.Log($"<color=green> [GameModeManager] Initializing game mode: {currentMode}</color>");

        var factionPanel = FactionPanelManager.Instance;

        if (factionPanel != null)
            factionPanel.OpenFactionPanel();
        else
            Debug.LogError("Faction Panel is null");

        switch (currentMode)
        {
            case Mode.Death_Solo:
                deathMatchSoloHandler.Initialize();
                break;
            case Mode.MultiPlayer_Type:
                break;
            case Mode.PVP_Type:
                break;
            case Mode.Scenario_Type:
                break;
        }

        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    private void OnClickClosePanel()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    public void ShowPanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy()
    {
        selectFaction.onClick.RemoveListener(OnClickSelectFaction);
        closeButton.onClick.RemoveListener(OnClickClosePanel);
    }
}


