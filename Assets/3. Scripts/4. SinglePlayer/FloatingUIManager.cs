using UnityEngine;
using System.Collections;

public class FloatingUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject floatingUIPrefab; // assign prefab with UI CanvasGroup + TileUIPanel
    private GameObject currentUI;
    private CanvasGroup currentGroup;
    private TileUIPanel currentPanel;
    public float fadeDuration = 1.0f; // seconds

    public void ShowUI(Tile tile)
    {
        // If we haven't created the UI yet, do it once
        if (currentUI == null)
        {
            currentUI = Instantiate(floatingUIPrefab, transform);
            currentPanel = currentUI.GetComponent<TileUIPanel>();
            currentGroup = currentUI.GetComponent<CanvasGroup>();
        }

        // Update panel content
        if (currentPanel != null)
            currentPanel.Open(tile);

        // Position above tile
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tile.transform.position + Vector3.up * 2f);
        RectTransform canvasRect = GetComponent<RectTransform>();
        RectTransform uiRect = currentUI.GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out localPos);
        uiRect.localPosition = localPos;

        // Fade in
        if (currentGroup != null)
        {
            StopAllCoroutines(); // stop any fade-out
            currentGroup.alpha = 0f;
            currentGroup.interactable = true;
            currentGroup.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(currentGroup, 0f, 1f));
        }
    }

    public void CloseUI()
    {
        if (currentUI != null && currentGroup != null)
        {
            StopAllCoroutines(); // stop any fade-in
            StartCoroutine(FadeCanvasGroup(currentGroup, currentGroup.alpha, 0f));
            currentGroup.interactable = false;
            currentGroup.blocksRaycasts = false;
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
        currentPanel = null;
    }
}
