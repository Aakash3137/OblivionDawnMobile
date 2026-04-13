using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RewardItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;

    public void Setup(RewardInstance reward)
    {
        icon.sprite = reward.rewardData.icon;
        nameText.text = reward.rewardData.rewardName;
        amountText.text = "x" + reward.amount;
    }
}