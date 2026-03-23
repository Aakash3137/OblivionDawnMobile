using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PanelType { CityCenter = 0, Units = 1, Buildings = 2 }

public class UpgradePanelNavigation : MonoBehaviour
{

    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private Toggle[] factionButtons;
    [Header("cardPanels: 0 = CityCenter ; 1 = Units ; 2 = Buildings")]
    [SerializeField] private Toggle[] typeButtons;
    private FactionName selectedFaction;
    [SerializeField] private Userdata userData;

    [Space(10)]
    [Header("UI References")]
    [SerializeField] private TMP_Text fragmentsCountText;

    private int selectedCategoryIndex;
    private CardsPanel currentCardPanel;

    private void Awake()
    {
        AddListeners();
    }

    private async Awaitable OnEnable()
    {
        await Awaitable.NextFrameAsync();
        SetCardPanelToOpen(FactionName.Futuristic, 1);
    }
    private async Awaitable Start()
    {
        await Awaitable.NextFrameAsync();
        typeButtons[0].group.allowSwitchOff = false;
        factionButtons[0].group.allowSwitchOff = false;
    }

    private void SetCardPanelToOpen(FactionName faction, int categoryIndex)
    {
        selectedFaction = faction;
        selectedCategoryIndex = categoryIndex;

        factionButtons[(int)faction].SetIsOnWithoutNotify(true);
        typeButtons[categoryIndex].SetIsOnWithoutNotify(true);

        // Debug.Log($"<color=green>[UpgradePanelNavigation] Activating {factionButtons[(int)faction].name} with category {typeButtons[categoryIndex].name} panel</color>");

        OnClickFaction(faction);
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

        userData.OnFragmentsChanged += UpdateFragmentsCount;
    }

    private void RemoveListeners()
    {
        foreach (Toggle toggle in factionButtons)
            toggle.onValueChanged.RemoveAllListeners();

        foreach (Toggle toggle in typeButtons)
            toggle.onValueChanged.RemoveAllListeners();

        userData.OnFragmentsChanged -= UpdateFragmentsCount;
    }

    private void OnClickFaction(FactionName faction)
    {
        selectedFaction = faction;

        // Debug.Log($"<color=green>[UpgradePanelNavigation] Faction set in UpgradePanelNavigation : {selectedFaction} clicked faction is {faction}</color>");

        ToggleFactionPanel(faction);

        OnClickCategory(selectedCategoryIndex);
    }

    private void OnClickCategory(int categoryIndex)
    {
        // Debug.Log($"<color=green>[UpgradePanelNavigation] calling OnClickCategory with index {categoryIndex}</color>");

        selectedCategoryIndex = categoryIndex;

        currentCardPanel = UpgradePanelManager.Instance.factionCardPanels[(int)selectedFaction].cardPanels[categoryIndex];
        ToggleTypePanel(currentCardPanel);

        UpdateFragmentsCount(selectedFaction);

        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void ToggleFactionPanel(FactionName faction)
    {
        var factionPanels = UpgradePanelManager.Instance.factionCardPanels;

        foreach (var factionPanel in factionPanels)
        {
            factionPanel.panelParent.SetActive(false);
        }

        factionPanels[(int)faction].panelParent.SetActive(true);
    }

    private void ToggleTypePanel(CardsPanel panel)
    {
        var factionPanel = UpgradePanelManager.Instance.factionCardPanels[(int)selectedFaction];

        foreach (var cardPanel in factionPanel.cardPanels)
            cardPanel.gameObject.SetActive(false);

        panel.gameObject.SetActive(true);

        panel.allCards[0].RefreshAllCards();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
    public void UpdateFragmentsCount(FactionName faction)
    {
        if (selectedFaction == faction)
            fragmentsCountText.SetText($"{userData.GetFragment((int)faction)}");

        var cardsInCurrentPanel = currentCardPanel.allCards;

        foreach (var card in cardsInCurrentPanel)
        {
            Identity identity = card.upgradeDataSO switch
            {
                BuildingDataSO buildingSO => buildingSO.buildingIdentity,
                UnitProduceStatsSO unitSO => unitSO.unitIdentity,
                _ => new Identity { }
            };

            if (identity.name != null || !identity.name.Equals(""))
                card.UpdateProgressBar(identity);
        }
    }
    public CardsPanel GetCurrentCardPanel() => currentCardPanel;
}
