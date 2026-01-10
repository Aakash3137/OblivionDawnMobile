using UnityEngine;
using System.Collections;

public class FadeHealthBar : MonoBehaviour
{
    public SpriteRenderer[] renderers;   // assign Background + FillBar here
    public float fadeDuration = 0.5f;    // how long to fade in/out
    public float visibleTime = 5f;       // how long to stay visible before fading
    public bool Isvisible;
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
        Isvisible = false;
    }

    public void ShowOnHit()
    {
        if (Isvisible) return; // already fully visible, skip

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
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
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t));
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
        Isvisible = true;


        // Wait 5 seconds AFTER LAST HIT
        yield return new WaitForSeconds(visibleTime);


        /*// 🔑 Check again before fading out
        if (Isvisible)
        {
            // Still marked visible (unit is still attacking), so skip fade‑out
            fadeRoutine = null;
            yield break;
        }*/


        // Fade out
        time = 0f;
        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t));
            }
            time += Time.deltaTime;
            yield return null;
        }

        // Invisible
        for (int i = 0; i < renderers.Length; i++)
        {
            Color c = originalColors[i];
            renderers[i].color = new Color(c.r, c.g, c.b, 0f);
        }
        Isvisible = false;
        fadeRoutine = null;
    }
}
