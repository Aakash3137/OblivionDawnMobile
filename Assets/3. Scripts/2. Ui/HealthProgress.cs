using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HealthProgress : ProgressManager
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    private MotionHandle fadeHandle;

    public void FadeOutHealthBar()
    {
        // Stop previous fade if still running
        fadeHandle.TryCancel();
        fadeHandle = LMotion.Create(1f, 0f, 1f)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(canvasGroup)
            .AddTo(this);
    }

    public void FadeInHealthBar()
    {
        // Stop previous fade if still running
        fadeHandle.TryCancel();
        fadeHandle = LMotion.Create(0f, 1f, 1f)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(canvasGroup)
            .AddTo(this);
    }
}
