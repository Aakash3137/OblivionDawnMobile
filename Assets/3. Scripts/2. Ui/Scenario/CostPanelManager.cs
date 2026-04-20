using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CostPanelManager : MonoBehaviour
{
    [SerializeField] private AllBuildingData allBuildingData;

    [Header("Texts")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    [Header("Roots")]
    [SerializeField] private GameObject foodRoot;
    [SerializeField] private GameObject goldRoot;
    [SerializeField] private GameObject metalRoot;
    [SerializeField] private GameObject powerRoot;

    [Header("Resource Icon Images")]
    [SerializeField] private Image foodIconImage;
    [SerializeField] private Image goldIconImage;
    [SerializeField] private Image metalIconImage;
    [SerializeField] private Image powerIconImage;

    private Sprite foodSprite;
    private Sprite goldSprite;
    private Sprite metalSprite;
    private Sprite powerSprite;

    private bool isVisible = false;

    private void Start()
    {
        GetResourceSprites();
        SetResourceSprites();
    }

    public void Show(BuildCost[] costs)
    {
        if (isVisible) return;

        SetCostValue(costs[0], foodText, foodRoot);
        SetCostValue(costs[1], goldText, goldRoot);
        SetCostValue(costs[2], metalText, metalRoot);
        SetCostValue(costs[3], powerText, powerRoot);

        gameObject.SetActive(true);

        isVisible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        isVisible = false;
    }

    private static void SetCostValue(BuildCost cost, TMP_Text text, GameObject root)
    {
        int value = cost.resourceAmount;

        if (value == 0)
        {
            if (root.activeSelf)
                root.SetActive(false);
            return;
        }

        if (!root.activeSelf)
            root.SetActive(true);

        text.SetText("{}", value);
    }

    #region  Resource Sprites
    private void GetResourceSprites()
    {
        FactionName currentFaction = GameData.playerFaction;

        foodSprite = allBuildingData.GetResourceBuildingsSO(currentFaction, ScenarioResourceType.Food)[0].buildingIcon;
        goldSprite = allBuildingData.GetResourceBuildingsSO(currentFaction, ScenarioResourceType.Gold)[0].buildingIcon;
        metalSprite = allBuildingData.GetResourceBuildingsSO(currentFaction, ScenarioResourceType.Metal)[0].buildingIcon;
        powerSprite = allBuildingData.GetResourceBuildingsSO(currentFaction, ScenarioResourceType.Power)[0].buildingIcon;
    }
    private void SetResourceSprites()
    {
        foodIconImage.sprite = foodSprite;
        goldIconImage.sprite = goldSprite;
        metalIconImage.sprite = metalSprite;
        powerIconImage.sprite = powerSprite;
    }
    #endregion
}
