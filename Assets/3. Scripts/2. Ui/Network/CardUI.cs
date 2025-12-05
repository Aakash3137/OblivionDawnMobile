/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("References")]
    public Image cardIcon;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Button cardButton;
    
    private CardData _cardData;
    
    public void Initialize(CardData cardData)
    {
        _cardData = cardData;
        
        if (cardIcon != null && cardData.cardIcon != null)
            cardIcon.sprite = cardData.cardIcon;
        
        if (cardNameText != null)
            cardNameText.text = cardData.cardName;
        
        if (costText != null)
            costText.text = $"G:{cardData.goldCost} W:{cardData.woodCost} O:{cardData.oilCost}";
        
        if (cardButton != null)
            cardButton.onClick.AddListener(OnCardClicked);
    }
    
    private void OnCardClicked()
    {
        if (NetworkCardManager.Instance != null)
        {
            NetworkCardManager.Instance.SelectCard(_cardData);
            Debug.Log($"[CardUI] Selected card: {_cardData.cardName}");
        }
    }
}
*/
