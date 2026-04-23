using UnityEngine;
using UnityEngine.UI;

public class FactionPanelManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private FactionScrollHandler fspManager;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        playButton.onClick.AddListener(OnClickPlayGame);
        closeButton.onClick.AddListener(OnClickClosePanel);
    }

    private void OnClickPlayGame()
    {
        var faction = fspManager.GetCurrentFaction();
        GameData.playerFaction = faction;

        GameStateManager.Instance.ChangeState(GameStateEnum.LOADING);
        Debug.Log($"<color=green>[FactionPanelManager] Selected Faction: {faction}</color>");

        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    private void OnClickClosePanel()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }
    public void OpenFactionPanel()
    {
        var factionCards = GetComponentsInChildren<FactionDisplayCard>();

        foreach(var card in factionCards)
        {
            card.RefreshCards();
        }

        ShowPanel();
    }

    private void ShowPanel()
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
        playButton.onClick.RemoveListener(OnClickPlayGame);
        closeButton.onClick.RemoveListener(OnClickClosePanel);
    }
}