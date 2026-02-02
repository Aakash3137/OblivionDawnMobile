using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject gem;
    public float gemYOffset = -0.5f;
    private RewardCanvas rewardCanvas;

    private void Awake()
    {
        rewardCanvas = RewardCanvas.Instance;
        if (rewardCanvas == null)
            Debug.Log("RewardCanvas is null");
    }

    public void SpawnGem()
    {
        var obj = Instantiate(gem, transform.position, Quaternion.identity, rewardCanvas.transform);
        obj.transform.forward = Camera.main.transform.forward;
        obj.transform.position += Vector3.up * gemYOffset;
    }
}
