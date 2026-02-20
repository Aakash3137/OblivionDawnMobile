using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HealthProgress : ProgressManager
{
    [SerializeField] private float fadeTime;
    [SerializeField] private float visibleTime;
    public bool isVisible;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        isVisible = false;
    }
    public void UpdateHealthBar()
    {
        if (isVisible)
            return;
        _ = FadeInAndOutAsync();
    }

    private async Awaitable FadeInAndOutAsync()
    {
        await LMotion.Create(0f, 1f, fadeTime)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(canvasGroup)
            .ToAwaitable(CancelBehavior.Complete, true, destroyCancellationToken);

        isVisible = true;

        await Awaitable.WaitForSecondsAsync(visibleTime);

        await LMotion.Create(1f, 0f, fadeTime)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(canvasGroup)
            .ToAwaitable(CancelBehavior.Complete, true, destroyCancellationToken);

        isVisible = false;
    }
}
