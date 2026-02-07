using TMPro;
using UnityEngine;

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
    [SerializeField] private UnityEngine.UI.Image foodIconImage;
    [SerializeField] private UnityEngine.UI.Image goldIconImage;
    [SerializeField] private UnityEngine.UI.Image metalIconImage;
    [SerializeField] private UnityEngine.UI.Image powerIconImage;

    void OnEnable()
    {
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

        foodGenerationRateText.SetText("{0}", RoundValue(rmReference.currentFoodGenerationRate));
        goldGenerationRateText.SetText("{0}", RoundValue(rmReference.currentGoldGenerationRate));
        metalGenerationRateText.SetText("{0}", RoundValue(rmReference.currentMetalGenerationRate));
        powerGenerationRateText.SetText("{0}", RoundValue(rmReference.currentPowerGenerationRate));

        // foodIconImage.sprite = rmReference.foodIcon;
        // goldIconImage.sprite = rmReference.goldIcon;
        // metalIconImage.sprite = rmReference.metalIcon;
        // powerIconImage.sprite = rmReference.powerIcon;

    }

    private void ToggleText(TMP_Text text, bool show)
    {
        var textGO = text.gameObject;
        textGO.SetActive(show);
    }
    private float RoundValue(float value)
    {
        return Mathf.Round(value * 100f) * 0.01f;
    }
}
