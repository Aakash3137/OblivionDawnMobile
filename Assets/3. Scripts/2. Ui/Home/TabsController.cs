using System.Collections;
using UnityEngine;

public class TabsController : MonoBehaviour
{
    public TabButton01[] tabButtons;
    public RectTransform[] panels;   // IMPORTANT: Use RectTransform for UI

    [Header("Animation Settings")]
    public float duration = 0.25f;

    private int currentIndex = -1;
    private bool isAnimating = false;

    void Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].Init(this, i);
        }

        SelectTab(2); // Default tab
    }

    public void SelectTab(int index)
    {
        if (currentIndex == index || isAnimating)
            return;

        int previousIndex = currentIndex;
        currentIndex = index;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            bool isSelected = i == index;
            tabButtons[i].SetSelected(isSelected);

            // Icon scale animation
            Transform iconTransform = tabButtons[i].transform.Find("Icon");
            if (iconTransform != null)
            {
                Vector3 targetScale = isSelected ? Vector3.one * 1.2f : Vector3.one * 0.8f;
                StartCoroutine(LerpScale(iconTransform, targetScale, 0.15f));
            }
        }

        if (previousIndex >= 0)
            StartCoroutine(SlidePanels(previousIndex, index));
        else
            panels[index].gameObject.SetActive(true);
    }

IEnumerator SlidePanels(int from, int to)
{
    isAnimating = true;

    float width = ((RectTransform)transform).rect.width;
    float duration = 0.35f;
    float time = 0f;

    bool movingForward = to > from;   // true = move left, false = move right
    float direction = movingForward ? -1f : 1f;

    int min = Mathf.Min(from, to);
    int max = Mathf.Max(from, to);

    // Activate all panels involved
    for (int i = min; i <= max; i++)
    {
        panels[i].gameObject.SetActive(true);
        panels[i].anchoredPosition = Vector2.zero;
    }

    while (time < duration)
    {
        time += Time.deltaTime;
        float t = Mathf.SmoothStep(0, 1, time / duration);

        for (int i = min; i <= max; i++)
        {
            float relativeIndexFrom = i - from;
            float relativeIndexTo = i - to;

            float startX = relativeIndexFrom * width;
            float targetX = relativeIndexTo * width;

            float x = Mathf.Lerp(startX, targetX, t);

            panels[i].anchoredPosition = new Vector2(x, 0);
        }

        yield return null;
    }

    // Reset everything
    for (int i = min; i <= max; i++)
    {
        panels[i].anchoredPosition = Vector2.zero;
        panels[i].gameObject.SetActive(i == to);
    }

    isAnimating = false;
}

    private IEnumerator LerpScale(Transform iconTransform, Vector3 targetScale, float duration)
    {
        Vector3 startScale = iconTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            iconTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        iconTransform.localScale = targetScale;
    }
}