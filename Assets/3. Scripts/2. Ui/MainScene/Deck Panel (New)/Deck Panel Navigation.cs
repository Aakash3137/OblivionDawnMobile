using UnityEngine;
using UnityEngine.UI;

public class DeckPanelNavigation : MonoBehaviour
{
    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private Toggle[] factionButtons;
    private bool[] isDeckDataLoaded = new bool[4];
    private bool isDefaultDataLoaded = false;

    private void Awake()
    {
        AddListeners();
    }
    private async Awaitable OnEnable()
    {
        await Awaitable.NextFrameAsync();
        SetDeckCardPanelToOpen(FactionName.Present);
    }
    private async Awaitable Start()
    {
        await Awaitable.NextFrameAsync();
        factionButtons[0].group.allowSwitchOff = false;
    }

    private void SetDeckCardPanelToOpen(FactionName faction)
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
        ToggleFactionPanel(faction);

        var dsmInstance = DeckSelectionManager.Instance;

        dsmInstance.selectedFaction = faction;

        if (!isDefaultDataLoaded)
        {
            dsmInstance.LoadDefaultData();
            isDefaultDataLoaded = true;
        }

        if (!isDeckDataLoaded[(int)faction])
        {
            dsmInstance.LoadDeckData();
            isDeckDataLoaded[(int)faction] = true;
        }

        dsmInstance.SetVariables();
        dsmInstance.RefreshSelectionCards();
    }

    private void ToggleFactionPanel(FactionName faction)
    {
        var factionPanels = DeckPanelManager.Instance.factionCardPanels;

        foreach (var factionPanel in factionPanels)
        {
            factionPanel.panelParent.SetActive(false);
        }

        factionPanels[(int)faction].panelParent.SetActive(true);
    }
}
