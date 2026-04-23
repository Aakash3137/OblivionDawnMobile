using UnityEngine;
using UnityEngine.UI;

public class DeckPanelNavigation : MonoBehaviour
{
    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private Toggle[] factionButtons;
    private bool[] isDeckDataLoaded = new bool[4];
    private bool isDefaultDataLoaded = false;
    private DeckPanelManager deckPanelManager;

    private void Start()
    {
        TryGetComponent(out deckPanelManager);
        AddListeners();
    }
    public void SetDeckCardPanelToOpen(FactionName faction)
    {
        factionButtons[(int)faction].SetIsOnWithoutNotify(true);
        OnClickFaction(faction);
    }

    private void AddListeners()
    {
        for (int i = 0; i < factionButtons.Length; i++)
        {
            FactionName faction = (FactionName)i;
            factionButtons[i].onValueChanged.AddListener((isOn) => { if (isOn) OnClickFaction(faction); });
        }
    }

    private void OnClickFaction(FactionName faction)
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        ToggleFactionPanel(faction);

        var dsmInstance = DeckSelectionManager.Instance;
        dsmInstance.selectedFaction = faction;

        var cityCenterData = deckPanelManager.GetMainBuildingUpgradeData(faction);
        // card panel index is 0 since no division into unit and building and city Center like upgrade panel
        var currentFactionCards = deckPanelManager.factionCardPanels[(int)faction].cardPanels[0].allCards;

        dsmInstance.InitializeDeckData(cityCenterData, currentFactionCards);
        dsmInstance.RefreshSelectionCards();

        if (!isDefaultDataLoaded)
        {
            dsmInstance.TryAddDefaultDeckData();
            isDefaultDataLoaded = true;
        }

        if (!isDeckDataLoaded[(int)faction])
        {
            dsmInstance.LoadDeckCardSelectionState(currentFactionCards);
            isDeckDataLoaded[(int)faction] = true;
        }
    }

    private void ToggleFactionPanel(FactionName faction)
    {
        var factionPanels = deckPanelManager.factionCardPanels;

        foreach (var factionPanel in factionPanels)
        {
            factionPanel.panelParent.SetActive(false);
        }

        factionPanels[(int)faction].panelParent.SetActive(true);
    }
}
