using UnityEngine;
using TMPro;

public class CostPanelManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    [Header("Roots")]
    [SerializeField] private GameObject foodRoot;
    [SerializeField] private GameObject goldRoot;
    [SerializeField] private GameObject metalRoot;
    [SerializeField] private GameObject powerRoot;

    private bool isVisible = false;

    public void Show(BuildCost[] costs)
    {
        if (isVisible) return;

        SetCostValue(costs[0], foodText, foodRoot);
        SetCostValue(costs[1], goldText, goldRoot);
        SetCostValue(costs[2], metalText, metalRoot);
        SetCostValue(costs[3], powerText, powerRoot);

        gameObject.SetActive(true);

        isVisible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        isVisible = false;
    }

    private static void SetCostValue(BuildCost cost, TMP_Text text, GameObject root)
    {
        int value = cost.resourceAmount;

        if (value == 0)
        {
            if (root.activeSelf)
                root.SetActive(false);
            return;
        }

        if (!root.activeSelf)
            root.SetActive(true);

        text.SetText("{}", value);
    }
}
