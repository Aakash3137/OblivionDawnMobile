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

    void OnEnable()
    {
        //Subscribe to the event
        if (prmReference != null)
            prmReference.OnResourcesChanged.AddListener(UpdateUI);
    }

    void OnDisable()
    {
        //Unsubscribe from the event
        if (prmReference != null)
            prmReference.OnResourcesChanged.RemoveListener(UpdateUI);
    }

    public void UpdateUI()
    {
        if (prmReference == null)
            return;
        foodText.text = prmReference.currentFood.ToString();
        goldText.text = prmReference.currentGold.ToString();
        metalText.text = prmReference.currentMetal.ToString();
        powerText.text = prmReference.CurrentPower.ToString();
    }
}
