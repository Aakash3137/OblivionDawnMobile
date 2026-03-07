using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PanelType { CityCenter = 0, Units = 1, Buildings = 2 }

public class UpgradePanelNavigation : MonoBehaviour
{
    public static UpgradePanelNavigation Instance { get; private set; }

    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private Toggle[] factionButtons;
    [Header("cardPanels: 0 = CityCenter ; 1 = Units ; 2 = Buildings")]
    [SerializeField] private Toggle[] typeButtons;
    private FactionName selectedFaction;
    [SerializeField] private Userdata userdata;

    [SerializeField] private TMP_Text fragmentsCountText;
    private int selectedCategoryIndex;
    private CardsPanel currentCardPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        AddListeners();
    }

    private async Awaitable OnEnable()
    {
        await Awaitable.NextFrameAsync();
        SetCardPanelToOpen(FactionName.Futuristic, 1);
    }

    private void SetCardPanelToOpen(FactionName faction, int categoryIndex)
    {
        selectedFaction = faction;
        selectedCategoryIndex = categoryIndex;

        factionButtons[(int)faction].isOn = true;
        typeButtons[categoryIndex].isOn = true;
    }

    private void AddListeners()
    {
        for (int i = 0; i < factionButtons.Length; i++)
        {
            FactionName faction = (FactionName)i;
            factionButtons[i].onValueChanged.AddListener((isOn) => { if (isOn) OnClickFaction(faction); });
        }
        for (int j = 0; j < typeButtons.Length; j++)
        {
            int index = j;
            typeButtons[index].onValueChanged.AddListener((isOn) => { if (isOn) OnClickCategory(index); });
        }
    }

    private void RemoveListeners()
    {
        foreach (Toggle toggle in factionButtons)
            toggle.onValueChanged.RemoveAllListeners();

        foreach (Toggle toggle in typeButtons)
            toggle.onValueChanged.RemoveAllListeners();
    }

    private void OnClickFaction(FactionName faction)
    {
        selectedFaction = faction;
        OnClickCategory(selectedCategoryIndex);
        UpdateFragmentsCount();
    }

    private void OnClickCategory(int categoryIndex)
    {
        selectedCategoryIndex = categoryIndex;
        currentCardPanel = UpgradePanelManager.Instance.factionCardPanel[(int)selectedFaction].cardPanels[categoryIndex];
        ToggleTypePanel(currentCardPanel);
        UpdateFragmentsCount();
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void ToggleTypePanel(CardsPanel panel)
    {
        foreach (var factionPanel in UpgradePanelManager.Instance.factionCardPanel)
        {
            foreach (var cardPanel in factionPanel.cardPanels)
                cardPanel.gameObject.SetActive(false);
        }

        panel.gameObject.SetActive(true);

        foreach (var card in panel.allCards)
            card.RefreshCards();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
    public void UpdateFragmentsCount() => fragmentsCountText.SetText($"{userdata.fragments[(int)selectedFaction]}");
    public CardsPanel GetCurrentCardPanel() => currentCardPanel;
}
