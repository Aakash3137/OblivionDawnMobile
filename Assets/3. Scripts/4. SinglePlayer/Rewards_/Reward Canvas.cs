using System.Collections.Generic;
using UnityEngine;

public class RewardCanvas : MonoBehaviour
{
    public static RewardCanvas Instance;
    public RewardUpdateUI rewardUpdateUI;

    public ScenarioGem gem;
    public float gemYOffset = -0.5f;

    [SerializeField] private List<ScenarioGem> rewards = new List<ScenarioGem>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddReward(Vector3 atPosition)
    {
        var obj = Instantiate(gem, atPosition, Quaternion.identity, transform);
        obj.transform.forward = Camera.main.transform.forward;
        obj.transform.position += Vector3.up * gemYOffset;
        rewards.Add(obj);
    }

    public void RemoveReward(ScenarioGem reward)
    {
        Destroy(reward.gameObject);
        rewards.Remove(reward);
    }

    public void ClearRewards()
    {
        foreach (var reward in rewards)
        {
            reward.OnGemClaim();
        }
    }
}
