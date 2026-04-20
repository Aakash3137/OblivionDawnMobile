using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialFlashController : MonoBehaviour
{
    public RectTransform flashCircle;
    public Image flashImage;

    [Header("Animation")]
    public float scaleDuration = 0.4f;
    public float fadeOutDuration = 0.5f;
    public float maxScale = 20f;

    public System.Action OnFlashPeak;

    private Coroutine routine;

    public void PlayFlash(Vector3 worldPosition)
    {
        Debug.Log("Flash catched...Starting animation!");

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FlashSequence(worldPosition));
    }

    private IEnumerator FlashSequence(Vector3 worldPos)
    {
        // Convert world → screen → UI position
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //flashCircle.position = screenPos;

        // Reset
        flashCircle.localScale = Vector3.zero;
        Color c = flashImage.color;
        c.a = 1f;
        flashImage.color = c;

        // SCALE UP (burst)
        float time = 0;
        while (time < scaleDuration)
        {
            time += Time.deltaTime;

            float t = time / scaleDuration;

            // Ease Out (fast burst)
            t = 1 - Mathf.Pow(1 - t, 3);

            float scale = Mathf.Lerp(0f, maxScale, t);
            flashCircle.localScale = Vector3.one * scale;

            yield return null;
        }

        // Peak moment → full screen white
        OnFlashPeak?.Invoke();

        // FADE OUT
        time = 0;
        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.SmoothStep(0, 1, time / fadeOutDuration);

            c.a = Mathf.Lerp(1f, 0f, t);
            flashImage.color = c;

            yield return null;
        }
    }
}