using TMPro;
using UnityEngine;

public class ResourceUpdateUI : MonoBehaviour
{
    [Header("Assign PlayerResourceManager Reference")]
    [SerializeField] private PlayerResourceManager prmReference;
    [Header("UI References assigned with Prefab")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    [SerializeField] private TMP_Text foodGenerationRateText;
    [SerializeField] private TMP_Text goldGenerationRateText;
    [SerializeField] private TMP_Text metalGenerationRateText;
    [SerializeField] private TMP_Text powerGenerationRateText;


    void OnEnable()
    {
        //Subscribe to the event
        if (prmReference != null)
            prmReference.OnResourcesChanged += UpdateUI;
    }

    void OnDisable()
    {
        //Unsubscribe from the event
        if (prmReference != null)
            prmReference.OnResourcesChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        if (prmReference == null)
            return;

        foodText.SetText("{0}", prmReference.currentFood);
        goldText.SetText("{0}", prmReference.currentGold);
        metalText.SetText("{0}", prmReference.currentMetal);
        powerText.SetText("{0}", prmReference.CurrentPower);

        ToggleText(foodGenerationRateText, prmReference.currentFoodGenerationRate > 0);
        ToggleText(goldGenerationRateText, prmReference.currentGoldGenerationRate > 0);
        ToggleText(metalGenerationRateText, prmReference.currentMetalGenerationRate > 0);
        ToggleText(powerGenerationRateText, prmReference.currentPowerGenerationRate > 0);

        foodGenerationRateText.SetText("{0}", RoundValue(prmReference.currentFoodGenerationRate));
        goldGenerationRateText.SetText("{0}", RoundValue(prmReference.currentGoldGenerationRate));
        metalGenerationRateText.SetText("{0}", RoundValue(prmReference.currentMetalGenerationRate));
        powerGenerationRateText.SetText("{0}", RoundValue(prmReference.currentPowerGenerationRate));
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
