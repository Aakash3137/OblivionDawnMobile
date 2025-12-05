using UnityEngine;
using System.Collections;

public class FloatingUIManager : MonoBehaviour
{
    public GameObject floatingUIPrefab; // assign prefab with world-space canvas
    private GameObject currentUI;
    private CanvasGroup currentGroup;
    public float fadeDuration = 1.5f; // seconds

    public void ShowUI(Tile tile)
    {
        // Destroy old UI if any
        if (currentUI != null) Destroy(currentUI);

        // Instantiate as child of this Canvas (since script is on Canvas)
        currentUI = Instantiate(floatingUIPrefab, transform);

        // Position relative to tile (convert world to screen)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tile.transform.position + Vector3.up * 2f);
        currentUI.GetComponent<RectTransform>().position = screenPos;

        // Fade in
        currentGroup = currentUI.GetComponent<CanvasGroup>();
        if (currentGroup != null)
        {
            currentGroup.alpha = 0f;
            StartCoroutine(FadeCanvasGroup(currentGroup, 0f, 1f));
        }
    }


    public void CloseUI()
    {
        if (currentUI != null && currentGroup != null)
        {
            StartCoroutine(FadeAndDestroy(currentGroup));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    private IEnumerator FadeAndDestroy(CanvasGroup cg)
    {
        yield return FadeCanvasGroup(cg, cg.alpha, 0f);
        Destroy(currentUI);
        currentUI = null;
        currentGroup = null;
    }

    void Update()
    {
        if (currentUI != null)
        {
            // Keep UI facing camera
            currentUI.transform.LookAt(Camera.main.transform);

            // Optional: lock rotation so it only faces forward (avoid flipping in orthographic)
            currentUI.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        }
    }
}
