using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelBox : MonoBehaviour
{
    [SerializeField] internal RewardBox RewardBoxPrefab;
    [SerializeField] internal int RewardQuantity = 0;
    [SerializeField] internal Image FillImage;
    [SerializeField] internal TMP_Text LevelNoTxt;

    public RewardBox SetReward(RewardData reward, Transform _Parent)
    {
        RewardBox R_Box = Instantiate(RewardBoxPrefab, _Parent);
        R_Box.RewardIcon.sprite = reward.RewardIcon;
        R_Box.RewardNameTxt.text = reward.RewardType.ToString();
        R_Box.ClaimButton.interactable = reward.RewardStatus;
        return R_Box;
    }
}
