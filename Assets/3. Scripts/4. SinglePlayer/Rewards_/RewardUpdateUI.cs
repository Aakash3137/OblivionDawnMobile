using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class RewardUpdateUI : MonoBehaviour
{
    public static RewardUpdateUI Instance { get; private set; }

    [SerializeField] private TMP_Text gemCountText;
    [SerializeField] private Transform gemIcon;

    public Userdata userdata;
    public GameObject gemClaimed;
    private Camera mainCamera;

    [SerializeField] private float animOffsetMin = 80f;
    [SerializeField] private float animOffsetMax = 120f;
    [SerializeField] private float animDurationDelay = 0.1f;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        mainCamera = Camera.main;
    }

    private void Start()
    {
        gemCountText.SetText("{0}", userdata.Diamonds);
    }

    public void UpdateGemCount(int amount)
    {
        //gemCountText.SetText($"{amount}");

        gemCountText.SetText("{0}", amount);
    }

    public void SpawnGemAndAnimate(Vector3 spawnPosition)
    {
        spawnPosition = mainCamera.WorldToScreenPoint(spawnPosition);

        var obj = Instantiate(gemClaimed, spawnPosition, Quaternion.identity, transform);

        Vector3 offsetPosition = Random.insideUnitCircle.normalized * Random.Range(animOffsetMin, animOffsetMax);
        Vector3 startPosition = spawnPosition;
        Vector3 endPosition = spawnPosition + offsetPosition;

        // Displace the gem a bit in random direction
        LMotion.Create(startPosition, endPosition, 0.3f)
            .WithEase(Ease.Linear).WithOnComplete(() =>
            {
                startPosition = endPosition;
                endPosition = gemIcon.position;
                // Animate the gem towards the gem icon
                Generic.Delay(() => GemAnimation(obj, startPosition, endPosition), animDurationDelay);
            })
            .BindToPosition(obj.GetComponent<RectTransform>())
            .AddTo(obj);
    }

    private void GemAnimation(GameObject obj, Vector3 startPosition, Vector3 endPosition)
    {
        LMotion.Create(startPosition, endPosition, 1f)
            .WithEase(Ease.InOutSine).WithOnComplete(() =>
            {
                Destroy(obj);
                userdata.Diamonds++;
                UpdateGemCount(userdata.Diamonds);
            })
            .BindToPosition(obj.GetComponent<RectTransform>())
            .AddTo(obj);
    }
}
