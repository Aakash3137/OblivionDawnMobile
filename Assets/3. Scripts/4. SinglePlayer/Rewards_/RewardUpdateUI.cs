using System.Collections;
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

        Vector3 offsetDirection = Random.insideUnitCircle.normalized * Random.Range(animOffsetMin, animOffsetMax);
        Vector3 startPosition = spawnPosition;
        Vector3 offsetPosition = spawnPosition + offsetDirection;
        Vector3 endPosition = gemIcon.position;

        // using Lit Motion sequence for animation
        LSequence.Create()
                .Append(LMotion.Create(startPosition, offsetPosition, 0.3f).WithEase(Ease.Linear).BindToPosition(obj.GetComponent<RectTransform>()))
                .AppendInterval(animDurationDelay)
                .Append(LMotion.Create(offsetPosition, endPosition, 1f).WithEase(Ease.InOutSine).WithOnComplete(() => OnGemCollected(obj)).BindToPosition(obj.GetComponent<RectTransform>()))
                .Run();
    }

    private void OnGemCollected(GameObject obj)
    {
        Destroy(obj);
        userdata.Diamonds++;
        UpdateGemCount(userdata.Diamonds);
    }
}
