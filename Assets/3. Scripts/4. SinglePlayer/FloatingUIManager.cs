using UnityEngine;
using System.Collections;

public class FloatingUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject floatingUIPrefab; // assign prefab with UI CanvasGroup
    private GameObject currentUI;
    private CanvasGroup currentGroup;
    public float fadeDuration = 1.5f; // seconds

    public void ShowUI(Tile tile)
    {
        if (currentUI != null) Destroy(currentUI);

        currentUI = Instantiate(floatingUIPrefab, transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(tile.transform.position + Vector3.up * 2f);

        RectTransform canvasRect = GetComponent<RectTransform>();
        RectTransform uiRect = currentUI.GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, Camera.main, out localPos);

        // Clamp inside canvas bounds
        Vector2 size = uiRect.sizeDelta;
        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        float minX = -canvasRect.rect.width / 2 + halfWidth;
        float maxX = canvasRect.rect.width / 2 - halfWidth;
        float minY = -canvasRect.rect.height / 2 + halfHeight;
        float maxY = canvasRect.rect.height / 2 - halfHeight;

        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        localPos.y = Mathf.Clamp(localPos.y, minY, maxY);

        uiRect.localPosition = localPos;

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

    // void Update()
    // {
    //     if (currentUI != null)
    //     {
    //         // Keep UI facing camera (for world-space canvases)
    //         currentUI.transform.LookAt(Camera.main.transform);

    //         // Optional: lock rotation so it only faces forward
    //         currentUI.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
    //     }
    // }



    void Update()
    {
        if (currentUI != null)
        {
            // If your FloatingUI prefab is inside a Screen Space Canvas,
            // you do NOT need to rotate it at all.
            // Just keep its anchored position updated if you want it to follow the tile.

            // Example: keep UI anchored above the tile even if camera moves
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                currentUI.GetComponentInParent<Tile>().transform.position + Vector3.up * 2f
            );

            RectTransform canvasRect = GetComponent<RectTransform>();
            RectTransform uiRect = currentUI.GetComponent<RectTransform>();

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, Camera.main, out localPos);

            uiRect.localPosition = localPos;
        }
    }

}
