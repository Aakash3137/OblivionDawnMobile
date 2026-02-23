using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    private RewardCanvas rewardCanvas;

    private void Awake()
    {
        rewardCanvas = RewardCanvas.Instance;
        if (rewardCanvas == null)
            Debug.Log("RewardCanvas is null");
    }

    public void SpawnGem()
    {
        rewardCanvas.AddReward(transform.position);
    }
}
