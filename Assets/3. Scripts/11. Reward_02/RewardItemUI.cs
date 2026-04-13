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
        //icon.sprite = reward.rewardData.icon;
        //if reward is a fragment, get faction-specific icon
        if(reward.rewardData.rewardType == RewardType02.Fragment || reward.rewardData.rewardType == RewardType02.Unit)
            icon.sprite = reward.rewardData.GetIcon(reward.faction);
        else
            icon.sprite = reward.rewardData.icon;

        nameText.text = reward.rewardData.rewardName;
        amountText.text = "+" + reward.amount;
    }
}