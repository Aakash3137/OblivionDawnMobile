using UnityEngine;
using System.Collections;

public class TapToOpenPulse : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    [Header("Timing")]
    public float duration = 1.0f;

    private RectTransform rectTransform;
    private Coroutine pulseRoutine;
    private bool isRunning = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartPulse();
    }

    public void StartPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        isRunning = true;
        pulseRoutine = StartCoroutine(PulseLoop());
    }

    public void StopPulse()
    {
        isRunning = false;

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        // Reset to normal scale
        rectTransform.localScale = Vector3.one;
    }

    private IEnumerator PulseLoop()
    {
        while (isRunning)
        {
            // Scale Up
            yield return Scale(minScale, maxScale);

            // Scale Down
            yield return Scale(maxScale, minScale);
        }
    }

    private IEnumerator Scale(float from, float to)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // Smooth easing (ease in-out)
            t = Mathf.SmoothStep(0, 1, t);

            float scale = Mathf.Lerp(from, to, t);
            rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }
    }
}