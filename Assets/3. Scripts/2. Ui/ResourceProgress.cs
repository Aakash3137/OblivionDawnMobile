using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;

public class ResourceProgress : ProgressManager
{
    private ResourceGenerationComponent resourceGenerationStats;
    private float currentTime;
    [SerializeField] private RectTransform generateResourceImageTransform;
    public float yOffset = 80f;
    private Image resourceImage;
    private Vector3 defaultTransform;

    private void Start()
    {
        resourceGenerationStats = GetComponentInParent<ResourceGenerationComponent>();
        currentTime = 0f;
        resourceImage = generateResourceImageTransform.GetComponent<Image>();
        defaultTransform = generateResourceImageTransform.anchoredPosition;

        AnimateTransform();
    }

    private void Update()
    {
        if (currentTime > resourceGenerationStats.resourceTimeToProduce)
        {
            AnimateTransform();
            currentTime = 0f;
        }

        currentTime += Time.deltaTime;
        progressAmount = currentTime / resourceGenerationStats.resourceTimeToProduce;
        UpdateFillAmount(progressAmount);
    }

    private void AnimateTransform()
    {
        generateResourceImageTransform.anchoredPosition = defaultTransform;

        Color alphaColor = resourceImage.color;
        alphaColor.a = 1f;
        resourceImage.color = alphaColor;
        alphaColor.a = 0f;

        // The starting and ending values
        float startY = generateResourceImageTransform.anchoredPosition.y;
        float endY = startY + yOffset;

        LMotion.Create(startY, endY, 1f)
            .WithEase(Ease.OutQuad).WithOnComplete(() =>
            {
                resourceImage.color = alphaColor;
                // Debug.Log("<color=green>Calling Animation Complete</color>");
            })
            .BindToAnchoredPositionY(generateResourceImageTransform)
            .AddTo(this);
    }
}
