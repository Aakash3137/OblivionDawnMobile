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
        for (int i = 0; i < rmReference.currentResources.Length; i++)
        {
            resourceTexts[i].SetText("{0}", rmReference.currentResources[i].resourceAmount);
        }
    }

    private void GenerationTextHandler()
    {
        for (int i = 0; i < generationRateTexts.Length; i++)
        {
            ToggleText(generationRateTexts[i], rmReference.currentGenerationRates[i].resourceAmount);
        }
    }

    private void UIStorageFillHandler()
    {
        float[] resourcePercentages = new float[rmReference.currentResources.Length];

        for (int i = 0; i < rmReference.currentResources.Length; i++)
        {
            resourcePercentages[i] = (float)rmReference.currentResources[i].resourceAmount / rmReference.maxResources[i].resourceAmount;
            UpdateFillAmount(resourceFillBars[i], resourcePercentages[i]);
        }
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
        if (resourceSprites.Length != resourceIconImages.Length)
        {
            Debug.Log($"<color=#000000>[ResourceUpdateUI] {gameObject.name} ResourceSprites from decSelectionData and ResourceIconImages on UI are not the same length</color>");
        }


        for (int i = 0; i < resourceIconImages.Length; i++)
        {
            resourceIconImages[i].sprite = resourceSprites[i];
        }
    }
    #endregion
}

