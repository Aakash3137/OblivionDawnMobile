using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FactionScrollPanelManager : MonoBehaviour
{
    private float mainCardScrollSpeed = 50f;
    private float cardScrollSpeed;
    [SerializeField] private float cardWidth;
    [SerializeField] private float cardOffset = 100f;
    [SerializeField] private float lastCardOffset = 30f;
    [Space(10)]

    private bool isDragging;
    private float totalDrag;
    [SerializeField] private bool wasDraggingLeft;

    Transform movingCard => transform.GetChild(movingCardIndex);

    private int totalCards;
    private int movingCardIndex;


    [SerializeField, ReadOnly] private List<Transform> previousOrder = new List<Transform>();


    private void Awake()
    {
        var factionCards = new List<FactionDisplayCard>(GetComponentsInChildren<FactionDisplayCard>());

        int factionCount = ScenarioDataTypes._factionEnumValues.Length;

        totalCards = factionCards.Count;
        movingCardIndex = totalCards - 1;

        for (int i = 0; i < totalCards; i++)
        {
            // if we there are duplicate cards loop them accordingly
            factionCards[i].faction = ScenarioDataTypes._factionEnumValues[i % factionCount];
        }

        cardWidth = factionCards[0].GetComponent<RectTransform>().rect.width;

        cardScrollSpeed = mainCardScrollSpeed * 0.05f;

        SnapCards();
    }

    private void Update()
    {
        // ScrollHandler();
    }

    private void ScrollHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            totalDrag = 0f;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            float dragDeltaX = Input.GetAxis("Mouse X");

            bool isDragLeft = dragDeltaX < 0;
            bool isDragRight = dragDeltaX > 0;

            totalDrag += dragDeltaX;

            var mainScroll = dragDeltaX * mainCardScrollSpeed;
            var cardScroll = dragDeltaX * cardScrollSpeed;

            movingCard.localPosition += new Vector3(mainScroll, 0, 0);

            if (isDragLeft)
            {
                // Dragging left
                if (movingCard.localPosition.x < -cardWidth * 0.95f)
                {
                    movingCard.SetAsFirstSibling();
                    SnapCards();
                }
            }
            else if (isDragRight)
            {
                // Dragging right
                if (movingCard.localPosition.x > 0.1f)
                {
                    transform.GetChild(0).SetAsLastSibling();
                }
                if (Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.2f)
                {
                    transform.GetChild(0).localPosition = new Vector3(-(cardWidth + lastCardOffset), 0, 0);
                }
                if (Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.1f)
                {
                    SnapCards();
                }
            }

            wasDraggingLeft = totalDrag < 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            if (!wasDraggingLeft && Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.6f)
            {
                SnapCards();
            }
            else if (wasDraggingLeft && Mathf.Abs(movingCard.localPosition.x) > cardWidth * 0.35f)
            {
                movingCard.SetAsFirstSibling();
                SnapCards();
            }
            else
                ResetOrder();
        }
    }

    private void SnapCards()
    {
        previousOrder.Clear();
        totalDrag = 0f;

        for (int i = 0; i < totalCards; i++)
        {
            var currentTransform = transform.GetChild(i);

            currentTransform.localPosition = new Vector3(cardOffset * (totalCards - 1 - i), 0, 0);

            if (i == 0)
            {
                currentTransform.localPosition = new Vector3(-(cardWidth + lastCardOffset), 0, 0);
            }

            previousOrder.Add(currentTransform);
        }
    }

    private void ResetOrder()
    {
        for (int i = 0; i < totalCards; i++)
        {
            var currentTransform = previousOrder[i];

            currentTransform.SetSiblingIndex(i);
        }

        SnapCards();
    }
}