using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionPanelScript : MonoBehaviour
{
    // [Header("Cards (0=Behind, 1=Left, 2=Right, 3=Front)")]
    // [SerializeField] private List<FactionCard> cards = new();

    // [Header("Animation")]
    // [SerializeField] private float moveDuration = 0.35f;

    // [Header("Scale")]
    // [SerializeField] private Vector3 normalScale = Vector3.one;
    // [SerializeField] private Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1.1f);

    // private readonly List<Vector2> slots = new(); // slot positions
    // private bool isMoving;

    // private Vector2 startTouch;

    // // ================================
    // // LIFE CYCLE
    // // ================================
    // private void OnEnable()
    // {
    //     CacheSlots();
    //     ApplyInstant();
    // }

    // public void OnClickShowHomePage()
    // {
    //     // HomeUIManager.Instance.ShowPanel(PanelName.Home);
    // }

    // // ================================
    // // CACHE SLOT POSITIONS (SIZE SAFE)
    // // ================================
    // private void CacheSlots()
    // {
    //     slots.Clear();

    //     // Force neutral scale so position cache is size-independent
    //     for (int i = 0; i < cards.Count; i++)
    //         cards[i].transform.localScale = normalScale;

    //     Canvas.ForceUpdateCanvases();

    //     for (int i = 0; i < cards.Count; i++)
    //     {
    //         RectTransform rt = cards[i].transform as RectTransform;
    //         slots.Add(rt.anchoredPosition);
    //     }
    // }

    // // ================================
    // // INPUT
    // // ================================
    // private void Update()
    // {
    //     if (isMoving) return;

    //     if (Input.GetMouseButtonDown(0))
    //         startTouch = Input.mousePosition;

    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         float deltaX = Input.mousePosition.x - startTouch.x;

    //         if (Mathf.Abs(deltaX) > 80f)
    //         {
    //             if (deltaX > 0) SwipeRight();
    //             else SwipeLeft();
    //         }
    //     }
    // }

    // // ================================
    // // SWIPE LOGIC
    // // ================================
    // public void SwipeLeft()
    // {
    //     if (isMoving) return;

    //     FactionCard first = cards[0];
    //     cards.RemoveAt(0);
    //     cards.Add(first);

    //     StartCoroutine(AnimateCards());
    // }

    // public void SwipeRight()
    // {
    //     if (isMoving) return;

    //     FactionCard last = cards[^1];
    //     cards.RemoveAt(cards.Count - 1);
    //     cards.Insert(0, last);

    //     StartCoroutine(AnimateCards());
    // }

    // // ================================
    // // ANIMATION
    // // ================================
    // private IEnumerator AnimateCards()
    // {
    //     isMoving = true;

    //     float t = 0f;

    //     Vector2[] startPos = new Vector2[cards.Count];
    //     Vector3[] startScale = new Vector3[cards.Count];

    //     for (int i = 0; i < cards.Count; i++)
    //     {
    //         RectTransform rt = cards[i].transform as RectTransform;
    //         startPos[i] = rt.anchoredPosition;
    //         startScale[i] = rt.localScale;
    //     }

    //     while (t < 1f)
    //     {
    //         t += Time.deltaTime / moveDuration;
    //         float eased = Mathf.SmoothStep(0f, 1f, t);

    //         for (int i = 0; i < cards.Count; i++)
    //         {
    //             RectTransform rt = cards[i].transform as RectTransform;

    //             rt.anchoredPosition = Vector2.Lerp(startPos[i], slots[i], eased);
    //             rt.localScale = Vector3.Lerp(
    //                 startScale[i],
    //                 (i == 3) ? selectedScale : normalScale,
    //                 eased
    //             );
    //         }

    //         yield return null;
    //     }

    //     ApplyInstant();
    //     isMoving = false;
    // }

    // // ================================
    // // APPLY FINAL STATE
    // // ================================
    // private void ApplyInstant()
    // {
    //     for (int i = 0; i < cards.Count; i++)
    //     {
    //         RectTransform rt = cards[i].transform as RectTransform;

    //         rt.anchoredPosition = slots[i];
    //         rt.localScale = (i == 3) ? selectedScale : normalScale;
    //         rt.SetSiblingIndex(i);

    //         cards[i].SetSelected(i == 3);
    //     }

    //     Debug.Log("Selected Faction: " + cards[3].factionId);
    // }
}
