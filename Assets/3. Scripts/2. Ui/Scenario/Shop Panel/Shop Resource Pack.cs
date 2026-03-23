using Sirenix.OdinInspector;
using UnityEngine;

public class ShopResourcePack : MonoBehaviour
{
    [Space(10), Range(0, 100)]
    [SerializeField] private float discount = 10f;
    [SerializeField] private int amount;
    [ReadOnly] public int cost;
    [Space(15)]
    [SerializeField] private ShopResourceCard[] resourceCards;

    private ResourceBuyButton buyButton;

    private async Awaitable Start()
    {
        await Awaitable.NextFrameAsync();
        cost = resourceCards.Length * Mathf.RoundToInt(amount * 0.03f);

        // apply discount
        cost = (int)(cost * (100f - discount) * 0.01f);

        foreach (ShopResourceCard card in resourceCards)
            card.SetAmountTextUI(amount);

        buyButton = GetComponentInChildren<ResourceBuyButton>();

        if (buyButton != null)
            buyButton.Initialize(cost, amount);
    }
}
