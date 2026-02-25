using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;

public class ResourceProgress : ProgressManager
{
    private ResourceBuildingStats resourceBuilding;
    private float currentTime;

    [SerializeField] private RectTransform generateResourceImageTransform;
    public float yOffset = 80f;
    private Image resourceImage;
    private Vector3 defaultTransform;

    float waitTime;


    private void Start()
    {
        resourceBuilding = GetComponentInParent<ResourceBuildingStats>();
        currentTime = 0f;
        resourceImage = generateResourceImageTransform.GetComponent<Image>();
        defaultTransform = generateResourceImageTransform.anchoredPosition;

        waitTime = resourceBuilding.GetGenerationTime();
        CheckBuildingSide();

        // AnimateTransform();
        resourceImage.color = new Color(resourceImage.color.r, resourceImage.color.g, resourceImage.color.b, 0f);
    }

    private void Update()
    {
        if (resourceBuilding != null)
            UIResourceBuildingProgress();
    }

    private void UIResourceBuildingProgress()
    {
        if (!resourceBuilding.isProducingResources)
        {
            currentTime = 0f;
            progressAmount = currentTime / waitTime;
            UpdateFillAmount(progressAmount);
            canvasGroup.alpha = 0f;
            return;
        }

        if (canvasGroup.alpha != 1f)
            canvasGroup.alpha = 1f;

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
        if (resourceBuilding.side == Side.Enemy)
        {
            canvasGroup.alpha = 0f;
        }
    }
}
