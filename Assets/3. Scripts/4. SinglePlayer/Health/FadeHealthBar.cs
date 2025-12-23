using UnityEngine;
using System.Collections;

public class FadeHealthBar : MonoBehaviour
{
    public SpriteRenderer[] renderers;   // assign Background + FillBar here
    public float fadeDuration = 1f;      // how long to fade in/out
    public float visibleTime = 5f;       // how long to stay visible before fading

    private Color[] originalColors;
    private Coroutine fadeRoutine;

    void Awake()
    {
        // Cache original colors and start invisible
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
            renderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0f);
        }
    }

    public void ShowOnHit()
    {
        // Cancel any ongoing fade
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        // Start fade in
        fadeRoutine = StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade in
        float time = 0f;
        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            for (int i = 0; i < renderers.Length; i++)
            {
                float alpha = Mathf.Lerp(0f, 1f, t);
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, alpha);
            }
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure fully visible
        for (int i = 0; i < renderers.Length; i++)
        {
            Color c = originalColors[i];
            renderers[i].color = new Color(c.r, c.g, c.b, 1f);
        }

        // Wait before fading out
        yield return new WaitForSeconds(visibleTime);

        // Fade out
        time = 0f;
        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            for (int i = 0; i < renderers.Length; i++)
            {
                float alpha = Mathf.Lerp(1f, 0f, t);
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, alpha);
            }
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure fully invisible
        for (int i = 0; i < renderers.Length; i++)
        {
            Color c = originalColors[i];
            renderers[i].color = new Color(c.r, c.g, c.b, 0f);
        }
    }
}
