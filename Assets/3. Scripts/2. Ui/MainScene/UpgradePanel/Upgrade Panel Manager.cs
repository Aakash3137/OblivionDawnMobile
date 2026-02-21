using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    public static UpgradePanelManager Instance;

    [SerializeField] private List<UnitProduceStatsSO> unitScriptables;
    [SerializeField] private List<BuildingDataSO> buildingScriptables;

    [SerializeField] public CardsPanel buildingCardPanel;
    [SerializeField] public CardsPanel unitCardPanel;

    [SerializeField] private CardUpgradeData cardPrefab;
    [field: SerializeField] public UpgradePopUpPanel upgradePopUpPanel { get; private set; }

    private void OnEnable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void OnDisable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        upgradePopUpPanel.gameObject.SetActive(false);

        if (!buildingCardPanel.initializedBuildings)
            CreateBuildingCards();

        if (!unitCardPanel.initializedUnits)
            CreateUnitCards();
    }

    public void CreateBuildingCards()
    {
        foreach (var buildingScriptable in buildingScriptables)
        {
            if (buildingScriptable == null)
            {
                Debug.Log("<color=Green> [Upgrade Panel Manager]Building Scriptable is null</color>");
                continue;
            }
            buildingCardPanel.AddCards(cardPrefab, buildingScriptable);
        }
    }

    public void CreateUnitCards()
    {
        foreach (var unitScriptable in unitScriptables)
        {
            if (unitScriptable == null)
            {
                Debug.Log("<color=Green> [Upgrade Panel Manager]Unit Scriptable is null</color>");
                continue;
            }
            unitCardPanel.AddCards(cardPrefab, unitScriptable);
        }
    }
}
