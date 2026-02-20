using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStackManager : MonoBehaviour
{
    [Header("Card Setup")]
    public List<GameObject> cards; // Assign your 4 cards
    public Vector3[] positions;    // Positions for slots 0 (front) to 3 (back)

    [Header("Animation Settings")]
    public float animationDuration = 0.3f;
    public float[] scales = { 1.2f, 1.0f, 0.8f, 0.6f };

    private bool isAnimating = false;
    private Vector2 fingerDownPos;
    private const float swipeThreshold = 50f;

    void Start()
    {
        UpdateCardVisuals(true);
    }

    void Update()
    {
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0))
            fingerDownPos = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
            DetectSwipe(Input.mousePosition);
    }

    private void DetectSwipe(Vector2 fingerUpPos)
    {
        float deltaX = fingerUpPos.x - fingerDownPos.x;

        if (Mathf.Abs(deltaX) > swipeThreshold)
            ShiftCards(deltaX < 0); // true = left swipe
    }

    private void ShiftCards(bool leftSwipe)
    {
        GameObject outgoingFrontCard = cards[0];

        if (leftSwipe)
        {
            // Move first card to back
            cards.RemoveAt(0);
            cards.Add(outgoingFrontCard);
        }
        else
        {
            // Bring last card to front
            GameObject incomingFrontCard = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            cards.Insert(0, incomingFrontCard);
        }

        // Update hierarchy so index 0 is always visually on top
        for (int i = 0; i < cards.Count; i++)
            cards[i].transform.SetSiblingIndex(cards.Count - 1 - i);

        StartCoroutine(AnimateTransition(outgoingFrontCard, leftSwipe));
    }

    private IEnumerator AnimateTransition(GameObject affectedCard, bool leftSwipe)
    {
        isAnimating = true;
        float elapsed = 0f;

        Vector3[] startPos = new Vector3[4];
        Vector3[] startScale = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            startPos[i] = cards[i].transform.localPosition;
            startScale[i] = cards[i].transform.localScale;
        }

        float offscreenX = -Screen.width * 1.2f;

        // If swipe RIGHT (bring last card forward)
        if (!leftSwipe)
        {
            // Place new front card offscreen left
            cards[0].transform.localPosition =
                new Vector3(offscreenX, positions[0].y, positions[0].z);

            SetAlpha(cards[0], 0f);
            startPos[0] = cards[0].transform.localPosition;
        }

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / animationDuration);

            for (int i = 0; i < 4; i++)
            {
                cards[i].transform.localPosition =
                    Vector3.Lerp(startPos[i], positions[i], t);

                cards[i].transform.localScale =
                    Vector3.Lerp(startScale[i], Vector3.one * scales[i], t);
            }

            if (leftSwipe)
            {
                // Fade OUT old front card
                float alpha = Mathf.Lerp(1f, 0f, t);
                SetAlpha(affectedCard, alpha);
            }
            else
            {
                // Fade IN new front card
                float alpha = Mathf.Lerp(0f, 1f, t);
                SetAlpha(cards[0], alpha);
            }

            yield return null;
        }

        UpdateCardVisuals(false);
        isAnimating = false;
    }

    private void UpdateCardVisuals(bool snapHierarchy)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.localPosition = positions[i];
            cards[i].transform.localScale = Vector3.one * scales[i];
            SetAlpha(cards[i], 1f);

            if (snapHierarchy)
                cards[i].transform.SetSiblingIndex(cards.Count - 1 - i);
        }
        
        UpdateStartButtonVisibility();
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = alpha;
            return;
        }

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void UpdateStartButtonVisibility()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Transform startBtn = cards[i].transform.Find("StartBtn");

            if (startBtn != null)
                startBtn.gameObject.SetActive(i == 0); // Only front card active
        }
    }
}
