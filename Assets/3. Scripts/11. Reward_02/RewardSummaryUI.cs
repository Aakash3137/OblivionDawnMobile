using UnityEngine;

public class RewardSummaryUI : MonoBehaviour
{
    public Transform gridContainer;
    public GameObject rewardItemPrefab;

    public void ShowSummary(RewardBundle bundle)
    {
        Clear();

        foreach (var reward in bundle.rewards)
        {
            GameObject obj = Instantiate(rewardItemPrefab, gridContainer);
            obj.GetComponent<RewardItemUI>().Setup(reward);
        }

        gameObject.SetActive(true);
    }

    private void Clear()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
    }
}