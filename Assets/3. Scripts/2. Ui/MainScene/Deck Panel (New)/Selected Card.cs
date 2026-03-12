using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCard : MonoBehaviour
{
    [ReadOnly] public ScriptableObject upgradeDataSO;
    [SerializeField] private Image cardImage;

    public bool isActive { get; private set; }

    private void Awake()
    {
        cardImage.gameObject.SetActive(false);
    }

    public void SetSelectedCard(ScriptableObject dataSO)
    {
        upgradeDataSO = dataSO;

        Sprite icon = dataSO switch
        {
            UnitProduceStatsSO unit => unit.unitIcon,
            BuildingDataSO building => building.buildingIcon,
            _ => null
        };

        if (icon == null)
        {
            Debug.LogWarning($"[SelectedCard] No icon found for SO type: {dataSO.GetType().Name}");
            return;
        }

        cardImage.sprite = icon;
        cardImage.gameObject.SetActive(true);
    }

    public void UnsetSelectedCard()
    {
        upgradeDataSO = null;
        cardImage.gameObject.SetActive(false);
    }

    public void ShowCard()
    {
        gameObject.SetActive(true);
        isActive = true;
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
        isActive = false;
    }
}