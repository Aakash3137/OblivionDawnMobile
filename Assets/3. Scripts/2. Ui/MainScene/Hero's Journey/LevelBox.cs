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
    [SerializeField] internal Image LockImage;

    public RewardBox SetReward(RewardInstance reward, Transform _Parent)
    {
        RewardBox R_Box = Instantiate(RewardBoxPrefab, _Parent);

        // NEW system mapping
        R_Box.RewardIcon.sprite = reward.rewardData.icon;
        R_Box.RewardNameTxt.text = reward.rewardData.rewardType.ToString();

        // Do NOT control interactable here anymore
        // (handled in LevelData based on lock + claim state)

        return R_Box;
    }
}
