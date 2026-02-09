using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUpdateUI : MonoBehaviour
{
    [Header("Assign PlayerResourceManager Reference")]
    [SerializeField] private ResourceManager rmReference;

    [Header("Resource Amount Texts")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    [Header("Generation Rate Texts")]
    [SerializeField] private TMP_Text foodGenerationRateText;
    [SerializeField] private TMP_Text goldGenerationRateText;
    [SerializeField] private TMP_Text metalGenerationRateText;
    [SerializeField] private TMP_Text powerGenerationRateText;


    [Header("Resource Icon Images")]
    [SerializeField] private Image foodIconImage;
    [SerializeField] private Image goldIconImage;
    [SerializeField] private Image metalIconImage;
    [SerializeField] private Image powerIconImage;

    [Header("Resource Icon FillBar")]
    [SerializeField] private Image foodFillBar;
    [SerializeField] private Image goldFillBar;
    [SerializeField] private Image metalFillBar;
    [SerializeField] private Image powerFillBar;

    private Sprite foodSprite;
    private Sprite goldSprite;
    private Sprite metalSprite;
    private Sprite powerSprite;

    void Start()
    {
        GetResourceSprites();
        //Subscribe to the event
        if (rmReference != null)
            rmReference.OnResourcesChanged += UpdateUI;
    }

    void OnDisable()
    {
        //Unsubscribe from the event
        if (rmReference != null)
            rmReference.OnResourcesChanged -= UpdateUI;
    }
    public void UpdateUI()
    {
        if (rmReference == null)
            return;

        foodText.SetText("{0}", rmReference.currentFood);
        goldText.SetText("{0}", rmReference.currentGold);
        metalText.SetText("{0}", rmReference.currentMetal);
        powerText.SetText("{0}", rmReference.CurrentPower);

        ToggleText(foodGenerationRateText, rmReference.currentFoodGenerationRate > 0);
        ToggleText(goldGenerationRateText, rmReference.currentGoldGenerationRate > 0);
        ToggleText(metalGenerationRateText, rmReference.currentMetalGenerationRate > 0);
        ToggleText(powerGenerationRateText, rmReference.currentPowerGenerationRate > 0);

        foodGenerationRateText.SetText("+{0}", rmReference.currentFoodGenerationRate);
        goldGenerationRateText.SetText("+{0}", rmReference.currentGoldGenerationRate);
        metalGenerationRateText.SetText("+{0}", rmReference.currentMetalGenerationRate);
        powerGenerationRateText.SetText("+{0}", rmReference.currentPowerGenerationRate);

        foodIconImage.sprite = foodSprite;
        goldIconImage.sprite = goldSprite;
        metalIconImage.sprite = metalSprite;
        powerIconImage.sprite = powerSprite;

        float foodPercent = (float)rmReference.currentFood / rmReference.maxFood;
        float goldPercent = (float)rmReference.currentGold / rmReference.maxGold;
        float metalPercent = (float)rmReference.currentMetal / rmReference.maxMetal;
        float powerPercent = (float)rmReference.CurrentPower / rmReference.maxPower;

        UpdateFillAmount(foodFillBar, foodPercent);
        UpdateFillAmount(goldFillBar, goldPercent);
        UpdateFillAmount(metalFillBar, metalPercent);
        UpdateFillAmount(powerFillBar, powerPercent);
    }

    private void ToggleText(TMP_Text text, bool show)
    {
        var textGO = text.GetComponent<CanvasGroup>();
        textGO.alpha = show ? 1f : 0f;
    }

    private void UpdateFillAmount(Image fillImage, float amount)
    {
        amount = Mathf.Clamp01(amount);
        fillImage.fillAmount = amount;
    }

    #region  Resource Sprites
    private void GetResourceSprites()
    {
        FactionName currentFaction = GameData.SelectedFaction;

        switch (currentFaction)
        {
            case FactionName.Medieval:
                SetMedievalSprites();
                break;
            case FactionName.Present:
                SetPresentSprites();
                break;
            case FactionName.Futuristic:
                SetFutureSprites();
                break;
            case FactionName.Galvadore:
                SetGalvadoreSprites();
                break;
        }
    }

    private void SetMedievalSprites()
    {
        var allFactionData = GameData.AllFactionsData;
        foodSprite = allFactionData.medievalFoodBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        goldSprite = allFactionData.medievalGoldBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        metalSprite = allFactionData.medievalMetalBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        powerSprite = allFactionData.medievalPowerBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
    }

    private void SetPresentSprites()
    {
        var allFactionData = GameData.AllFactionsData;
        foodSprite = allFactionData.presentFoodBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        goldSprite = allFactionData.presentGoldBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        metalSprite = allFactionData.presentMetalBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        powerSprite = allFactionData.presentPowerBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
    }

    private void SetFutureSprites()
    {
        var allFactionData = GameData.AllFactionsData;
        foodSprite = allFactionData.futureFoodBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        goldSprite = allFactionData.futureGoldBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        metalSprite = allFactionData.futureMetalBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        powerSprite = allFactionData.futurePowerBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
    }

    private void SetGalvadoreSprites()
    {
        var allFactionData = GameData.AllFactionsData;
        foodSprite = allFactionData.galvadoreFoodBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        goldSprite = allFactionData.galvadoreGoldBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        metalSprite = allFactionData.galvadoreMetalBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
        powerSprite = allFactionData.galvadorePowerBuilding.GetComponent<BuildingStats>().buildingStats.buildingIcon;
    }
    #endregion
}

