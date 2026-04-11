using UnityEngine;

public class DailyRewardButton : MonoBehaviour
{
    public RewardBundle dailyReward;

    public void OnClickClaim()
    {
        RewardManager.Instance.ClaimReward(dailyReward);
    }
}