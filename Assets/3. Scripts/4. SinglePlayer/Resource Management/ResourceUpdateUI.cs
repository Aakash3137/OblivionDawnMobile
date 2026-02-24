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

    private void OnEnable()
    {
        if (rmReference != null)
        {
            rmReference.OnResourcesChanged += UpdateUI;
            Debug.Log("[ResourceUpdateUI] Subscribed to OnResourcesChanged event for " + gameObject.name);
        }
    }
    private void Start()
    {
        GetResourceSprites();
        SetResourceSprites();
    }

    private void OnDisable()
    {
        //Unsubscribe from the event
        if (rmReference != null)
            rmReference.OnResourcesChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        if (rmReference == null)
            return;

        ResourceTextHandler();
        GenerationTextHandler();
        UIStorageFillHandler();
    }

    private void ResourceTextHandler()
    {
        foodText.SetText("{0}", rmReference.currentFood);
        goldText.SetText("{0}", rmReference.currentGold);
        metalText.SetText("{0}", rmReference.currentMetal);
        powerText.SetText("{0}", rmReference.CurrentPower);
    }

    private void GenerationTextHandler()
    {
        ToggleText(foodGenerationRateText, rmReference.currentFoodGenerationRate);
        ToggleText(goldGenerationRateText, rmReference.currentGoldGenerationRate);
        ToggleText(metalGenerationRateText, rmReference.currentMetalGenerationRate);
        ToggleText(powerGenerationRateText, rmReference.currentPowerGenerationRate);
    }

    private void UIStorageFillHandler()
    {
        float foodPercent = (float)rmReference.currentFood / rmReference.maxFood;
        float goldPercent = (float)rmReference.currentGold / rmReference.maxGold;
        float metalPercent = (float)rmReference.currentMetal / rmReference.maxMetal;
        float powerPercent = (float)rmReference.CurrentPower / rmReference.maxPower;

        UpdateFillAmount(foodFillBar, foodPercent);
        UpdateFillAmount(goldFillBar, goldPercent);
        UpdateFillAmount(metalFillBar, metalPercent);
        UpdateFillAmount(powerFillBar, powerPercent);
    }

    private void ToggleText(TMP_Text text, float amount)
    {
        bool show = amount != 0;
        var textGO = text.GetComponent<CanvasGroup>();
        textGO.alpha = show ? 1f : 0f;

        if (amount > 0)
            text.SetText("+{0}", amount);
        else if (amount < 0)
            text.SetText("{0}", amount);
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

    private void SetResourceSprites()
    {
        foodIconImage.sprite = foodSprite;
        goldIconImage.sprite = goldSprite;
        metalIconImage.sprite = metalSprite;
        powerIconImage.sprite = powerSprite;
    }
    #endregion
}

