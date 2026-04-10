using UnityEngine;
using System.Collections;

public class UIAnimator : MonoBehaviour
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Coroutine currentAnim;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Play(UIAnimPresetSO preset)
    {
        if (preset == null) return;

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(Animate(preset));
    }

    private IEnumerator Animate(UIAnimPresetSO preset)
    {
        float time = 0f;

        Vector3 initialScale = preset.useScale ? preset.startScale : rect.localScale;
        Vector3 targetScale = preset.endScale;

        float initialAlpha = preset.startAlpha;
        float targetAlpha = preset.endAlpha;

        if (preset.useScale)
            rect.localScale = initialScale;

        if (preset.useFade)
            canvasGroup.alpha = initialAlpha;

        while (time < preset.duration)
        {
            float t = time / preset.duration;
            float eval = preset.curve.Evaluate(t);

            if (preset.useScale)
                rect.localScale = Vector3.LerpUnclamped(initialScale, targetScale, eval);

            if (preset.useFade)
                canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, eval);

            time += Time.deltaTime;
            yield return null;
        }

        if (preset.useBounce)
            yield return Bounce(preset);

        rect.localScale = targetScale;
        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator Bounce(UIAnimPresetSO preset)
    {
        float time = 0.1f;
        float t = 0f;

        Vector3 overshoot = preset.endScale * preset.bounceStrength;

        while (t < time)
        {
            rect.localScale = Vector3.Lerp(preset.endScale, overshoot, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        while (t < time)
        {
            rect.localScale = Vector3.Lerp(overshoot, preset.endScale, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }
}