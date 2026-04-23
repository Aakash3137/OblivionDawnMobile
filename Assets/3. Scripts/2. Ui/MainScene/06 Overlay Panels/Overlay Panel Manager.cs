using UnityEngine;
public class OverlayPanelManager : MonoBehaviour
{
    public static OverlayPanelManager Instance { get; private set; }

    [field: SerializeField] public UpgradePopUpPanel upgradePopUpPanel { get; private set; }
    [field: SerializeField] public HeroJourneyPanelManager heroJourneyPanel { get; private set; }
    [field: SerializeField] public FactionPanelManager factionPanel { get; private set; }
    [field: SerializeField] public RewardPanelScript rewardPanel { get; private set; }
    [field: SerializeField] public GameModeManager gameModePanel { get; private set; }
    [field: SerializeField] public ProfileManager profilePanel { get; private set; }
    [field: SerializeField] public SettingPanelScript settingPanel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DisableAllPanels();
    }

    private void DisableAllPanels()
    {
        upgradePopUpPanel.HidePanel();
        heroJourneyPanel.HidePanel();
        factionPanel.HidePanel();
        rewardPanel.HidePanel();
        gameModePanel.HidePanel();
        profilePanel.HidePanel();
        settingPanel.HidePanel();
    }
}
