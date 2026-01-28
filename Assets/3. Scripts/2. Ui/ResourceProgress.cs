using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;

public class ResourceProgress : ProgressManager
{
    private ResourceBuildingStats resourceBuildingStats;
    private float currentTime;

    [SerializeField] private RectTransform generateResourceImageTransform;
    public float yOffset = 80f;
    private Image resourceImage;
    private Vector3 defaultTransform;

    private CanvasGroup _canvasGroup;

    float waitTime;

    private void Start()
    {
        resourceBuildingStats = GetComponentInParent<ResourceBuildingStats>();
        currentTime = 0f;
        resourceImage = generateResourceImageTransform.GetComponent<Image>();
        defaultTransform = generateResourceImageTransform.anchoredPosition;

        waitTime = resourceBuildingStats.GetGenerationTime();

        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        _canvasGroup = GetComponent<CanvasGroup>();
        CheckBuildingSide();
        AnimateTransform();
    }

    private void Update()
    {
        if (resourceBuildingStats != null)
            UIResourceBuildingProgress();
    }

    private void UIResourceBuildingProgress()
    {
        if (currentTime > waitTime)
        {
            AnimateTransform();
            currentTime = 0f;
        }

        currentTime += Time.deltaTime;
        progressAmount = currentTime / waitTime;
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
    
    //Disable UI for enemy
    private void CheckBuildingSide()
    {
        if (resourceBuildingStats.side == Side.Enemy)
        {
            _canvasGroup.alpha = 0f;
        }
    }
}
