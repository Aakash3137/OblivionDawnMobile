using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBuyButton : MonoBehaviour
{
    [SerializeField] private Userdata userdata;

    private Button button;
    private TMP_Text buttonText;

    private int cost;
    private int amount;
    private int resourceIndex;


    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();

        var card = GetComponentInParent<ShopResourceCard>();
        var pack = GetComponentInParent<ShopResourcePack>();

        if (card == null && pack == null)
        {
            button.onClick.AddListener(OnClickBalanceResources);

            cost = 5;
            buttonText.SetText($"{cost}");
            SetButtonInteractivity(userdata.Diamonds);
        }

        if (userdata != null)
            userdata.OnDiamondsChanged += SetButtonInteractivity;

    }

    private void SetButtonInteractivity(int diamonds)
    {
        button.interactable = diamonds >= cost;
    }

    public void Initialize(int cost, int amount, int resourceIndex = -1)
    {
        this.cost = cost;
        this.amount = amount;
        this.resourceIndex = resourceIndex;

        buttonText.SetText($"{cost}");
        button.onClick.AddListener(OnClickBuy);

        SetButtonInteractivity(userdata.Diamonds);
    }

    private void OnClickBalanceResources()
    {
        PlayerResourceManager.Instance.BalanceResources();
        userdata.Diamonds -= cost;
    }

    private void OnClickBuy()
    {
        if (resourceIndex < 0)
        {
            PlayerResourceManager.Instance.AddResources(amount);
        }
        else
        {
            PlayerResourceManager.Instance.AddResources((ScenarioResourceType)resourceIndex, amount);
        }
        userdata.Diamonds -= cost;
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickBalanceResources);
        button.onClick.RemoveListener(OnClickBuy);

        if (userdata != null)
            userdata.OnDiamondsChanged -= SetButtonInteractivity;
    }
}
