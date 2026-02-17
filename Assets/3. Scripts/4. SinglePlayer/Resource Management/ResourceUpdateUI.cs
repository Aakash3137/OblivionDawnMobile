using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUpdateUI : MonoBehaviour
{

    [Header("Assign PlayerResourceManager Reference")]
    [SerializeField] private ResourceManager rmReference;

    [SerializeField] private DecSelectionData decSelectionData;

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
        if (decSelectionData == null)
        {
            Debug.Log("<color=#000000>[ResourceUpdateUI] DecSelectionData is null</color>");
            return;
        }

        FactionName currentFaction = decSelectionData.CurrentFaction;

        foodSprite = decSelectionData.AllFactionDecData[(int)currentFaction].SelectedResourceDeck[0].buildingIcon;
        goldSprite = decSelectionData.AllFactionDecData[(int)currentFaction].SelectedResourceDeck[1].buildingIcon;
        metalSprite = decSelectionData.AllFactionDecData[(int)currentFaction].SelectedResourceDeck[2].buildingIcon;
        powerSprite = decSelectionData.AllFactionDecData[(int)currentFaction].SelectedResourceDeck[3].buildingIcon;
    }
    #endregion
}

