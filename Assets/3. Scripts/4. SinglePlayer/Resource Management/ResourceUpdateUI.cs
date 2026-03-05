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
    [SerializeField] private TMP_Text[] resourceTexts;

    [Header("Generation Rate Texts")]
    [SerializeField] private TMP_Text[] generationRateTexts;
    private CanvasGroup[] generationRateTextCanvasGroups;

    [Header("Building Count Texts")]
    [SerializeField] private TMP_Text[] buildingCountTexts;

    [Header("Resource Icon Images")]
    [SerializeField] private Image[] resourceIconImages;

    [Header("Resource Icon FillBar")]
    [SerializeField] private Image[] resourceFillBars;

    private Sprite[] resourceSprites;


    private void OnEnable()
    {
        if (rmReference != null)
        {
            rmReference.OnResourcesChanged += UpdateUI;
            Debug.Log("[ResourceUpdateUI] Subscribed to OnResourcesChanged event for " + gameObject.name);
        }
    }

    private void Awake()
    {
        if (generationRateTextCanvasGroups == null || generationRateTextCanvasGroups.Length != generationRateTexts.Length)
        {
            generationRateTextCanvasGroups = new CanvasGroup[generationRateTexts.Length];

            for (int i = 0; i < generationRateTexts.Length; i++)
            {
                generationRateTextCanvasGroups[i] = generationRateTexts[i].GetComponent<CanvasGroup>();
            }
        }
    }

    private void Start()
    {
        GetResourceSprites();
        SetResourceSprites();
    }

    private void OnDestroy()
    {
        if (rmReference != null)
        {
            rmReference.OnResourcesChanged -= UpdateUI;
            // Debug.Log("[ResourceUpdateUI] Unsubscribed from OnResourcesChanged event for " + gameObject.name);
        }
    }

    public void UpdateUI()
    {
        if (rmReference == null)
            return;

        ResourceTextHandler();
        GenerationTextHandler();
        BuildingCountTextHandler();
        UIStorageFillHandler();
    }

    private void ResourceTextHandler()
    {
        for (int i = 0; i < rmReference.currentResources.Length; i++)
        {
            resourceTexts[i].SetText("{0}", rmReference.currentResources[i].resourceAmount);
        }
    }

    private void GenerationTextHandler()
    {
        for (int i = 0; i < generationRateTexts.Length; i++)
        {
            ToggleText(generationRateTexts[i], generationRateTextCanvasGroups[i], rmReference.currentGenerationRates[i].resourceAmount);
        }
    }

    private void BuildingCountTextHandler()
    {
        for (int i = 0; i < buildingCountTexts.Length; i++)
        {
            buildingCountTexts[i].SetText("{0}", rmReference.resourceBuildingCounts[i]);
        }
    }

    private void UIStorageFillHandler()
    {
        float[] resourcePercentages = new float[rmReference.currentResources.Length];

        for (int i = 0; i < rmReference.currentResources.Length; i++)
        {
            if (rmReference.maxResources[i].resourceAmount > 0)
                resourcePercentages[i] = (float)rmReference.currentResources[i].resourceAmount / rmReference.maxResources[i].resourceAmount;
            else
                resourcePercentages[i] = 0f;
            UpdateFillAmount(resourceFillBars[i], resourcePercentages[i]);
        }
    }

    private void ToggleText(TMP_Text text, CanvasGroup textCanvas, float amount)
    {
        textCanvas.alpha = amount != 0 ? 1f : 0f;

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

        var decFactionData = decSelectionData.AllFactionDecData[(int)decSelectionData.CurrentFaction];
        var totalResources = decFactionData.SelectedResourceDeck.Count;

        resourceSprites = new Sprite[totalResources];

        for (int i = 0; i < totalResources; i++)
        {
            resourceSprites[i] = decFactionData.SelectedResourceDeck[i].buildingIcon;
        }
    }

    private void SetResourceSprites()
    {
        if (resourceSprites == null || resourceSprites.Length != resourceIconImages.Length)
        {
            Debug.Log($"<color=#000000>[ResourceUpdateUI] {gameObject.name} ResourceSprites from decSelectionData and ResourceIconImages on UI are not the same length</color>");
            return;
        }

        for (int i = 0; i < resourceIconImages.Length; i++)
        {
            resourceIconImages[i].sprite = resourceSprites[i];
        }
    }
    #endregion
}

