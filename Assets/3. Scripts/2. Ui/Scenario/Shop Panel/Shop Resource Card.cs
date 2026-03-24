using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopResourceCard : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private ScenarioResourceType resourceType;
    [SerializeField] private int amount;
    [ReadOnly] public int cost;
    [Space(15)]
    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private TMP_Text resourceName;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image resourceImage;
    private ResourceBuildingDataSO resourceSO;

    private ResourceBuyButton buyButton;

    private void Start()
    {
        resourceSO = allBuildingData.GetResourceBuildingSO(GameData.playerFaction, resourceType);

        resourceImage.sprite = resourceSO.buildingIcon;
        resourceName.SetText($"{resourceSO.buildingIdentity.name}");

        SetAmountTextUI(amount);

        cost = Mathf.RoundToInt(amount * 0.03f);

        buyButton = GetComponentInChildren<ResourceBuyButton>();

        if (buyButton != null)
            buyButton.Initialize(cost, amount, (int)resourceType);
    }

    public void SetAmountTextUI(int amount)
    {
        amountText.SetText($"x{amount}");
    }
}
