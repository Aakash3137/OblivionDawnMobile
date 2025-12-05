/*
using UnityEngine;

public class CardDeckUI : MonoBehaviour
{
    [Header("References")]
    public GameObject cardUIPrefab;
    public Transform cardContainer;
    public CardData[] playerDeck;
    
    private void Start()
    {
        InitializeDeck();
    }
    
    private void InitializeDeck()
    {
        foreach (var cardData in playerDeck)
        {
            GameObject cardObj = Instantiate(cardUIPrefab, cardContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(cardData);
            }
        }
    }
}
*/
