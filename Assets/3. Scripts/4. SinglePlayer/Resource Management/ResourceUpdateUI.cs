using TMPro;
using UnityEngine;

public class ResourceUpdateUI : MonoBehaviour
{
    [Header("Assign PlayerResourceManager Reference")]
    [SerializeField] private ResourceManager rmReference;
    [Header("UI References assigned with Prefab")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    [SerializeField] private TMP_Text foodGenerationRateText;
    [SerializeField] private TMP_Text goldGenerationRateText;
    [SerializeField] private TMP_Text metalGenerationRateText;
    [SerializeField] private TMP_Text powerGenerationRateText;

    // private float startFood;
    // private float startGold;
    // private float startMetal;
    // private float startPower;


    private void OnEnable()
    {
        //Subscribe to the event
        if (rmReference != null)
            rmReference.OnResourcesChanged += UpdateUI;
    }

    private void OnDisable()
    {
        //Unsubscribe from the event
        if (rmReference != null)
            rmReference.OnResourcesChanged -= UpdateUI;
    }

    // private void Start()
    // {
    //     // startFood = rmReference.startingResources[0].resourceCost;
    //     // startGold = rmReference.startingResources[1].resourceCost;
    //     // startMetal = rmReference.startingResources[2].resourceCost;
    //     // startPower = rmReference.startingResources[3].resourceCost;
    // }

    public void UpdateUI()
    {
        if (rmReference == null)
            return;

        foodText.SetText("{0:0} / {1:0}", rmReference.currentFood, rmReference.maxFood);
        goldText.SetText("{0:0} / {1:0}", rmReference.currentGold, rmReference.maxGold);
        metalText.SetText("{0:0} / {1:0}", rmReference.currentMetal, rmReference.maxMetal);
        powerText.SetText("{0:0} / {1:0}", rmReference.CurrentPower, rmReference.maxPower);

        ToggleText(foodGenerationRateText, rmReference.currentFoodGenerationRate > 0);
        ToggleText(goldGenerationRateText, rmReference.currentGoldGenerationRate > 0);
        ToggleText(metalGenerationRateText, rmReference.currentMetalGenerationRate > 0);
        ToggleText(powerGenerationRateText, rmReference.currentPowerGenerationRate > 0);

        foodGenerationRateText.SetText("{0}", RoundValue(rmReference.currentFoodGenerationRate));
        goldGenerationRateText.SetText("{0}", RoundValue(rmReference.currentGoldGenerationRate));
        metalGenerationRateText.SetText("{0}", RoundValue(rmReference.currentMetalGenerationRate));
        powerGenerationRateText.SetText("{0}", RoundValue(rmReference.currentPowerGenerationRate));
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
