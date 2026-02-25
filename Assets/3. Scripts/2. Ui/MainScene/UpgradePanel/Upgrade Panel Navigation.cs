using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelNavigation : MonoBehaviour
{
    [Space(20)]
    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private List<Toggle> factionButtons;
    [Header("Category Buttons : 0 = Units ; 1 = Buildings")]
    [SerializeField] private List<Toggle> categoryButtons;

    private UpgradePanelManager upgradePanelManager;
    private FactionName selectedFaction;

    public bool isDefaultPanelUnits = true;

    private void Start()
    {
        upgradePanelManager = UpgradePanelManager.Instance;
        AddListeners();

        factionButtons[0].isOn = true;
    }

    private void AddListeners()
    {
        factionButtons[0].onValueChanged.AddListener((isOn) => OnClickMedieval());
        factionButtons[1].onValueChanged.AddListener((isOn) => OnClickPresent());
        factionButtons[2].onValueChanged.AddListener((isOn) => OnClickFuture());
        factionButtons[3].onValueChanged.AddListener((isOn) => OnClickGalvadore());

        categoryButtons[0].onValueChanged.AddListener((isOn) => OnClickUnits(selectedFaction));
        categoryButtons[1].onValueChanged.AddListener((isOn) => OnClickBuildings(selectedFaction));

    }
    private void RemoveListeners()
    {
        foreach (Toggle toggle in factionButtons)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
        foreach (Toggle toggle in categoryButtons)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

    }

    private void OnClickMedieval()
    {
        selectedFaction = FactionName.Medieval;
        categoryButtons[0].isOn = isDefaultPanelUnits;
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private void OnClickPresent()
    {
        selectedFaction = FactionName.Present;
        categoryButtons[0].isOn = isDefaultPanelUnits;
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private void OnClickFuture()
    {
        selectedFaction = FactionName.Futuristic;
        categoryButtons[0].isOn = isDefaultPanelUnits;
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private void OnClickGalvadore()
    {
        selectedFaction = FactionName.Galvadore;
        categoryButtons[0].isOn = isDefaultPanelUnits;
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void OnClickUnits(FactionName factionName)
    {
        ToggleTypePanel(upgradePanelManager.unitCardPanel.gameObject);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private void OnClickBuildings(FactionName factionName)
    {
        ToggleTypePanel(upgradePanelManager.buildingCardPanel.gameObject);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void ToggleTypePanel(GameObject panel)
    {
        upgradePanelManager.buildingCardPanel.gameObject.SetActive(false);
        upgradePanelManager.unitCardPanel.gameObject.SetActive(false);

        panel.SetActive(true);
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}
