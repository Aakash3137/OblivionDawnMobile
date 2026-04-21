using System.Collections.Generic;
using UnityEngine;

public class FactionScrollPanelManager : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private float mainCardScrollSpeed = 50f;
    [SerializeField] private float cardScrollSpeed = 3.2f;
    [SerializeField] private float cardWidth = 700f;
    [SerializeField] private float cardOffset = 100f;
    [SerializeField] private float lastCardOffset = 30f;
    [SerializeField] private RectTransform scrollArea;

    private bool isDragging;
    private float totalDrag;
    private bool wasDraggingLeft;
    private bool wasDraggingRight;

    private int totalCards;

    private int movingCardIndex;
    Transform movingCard => transform.GetChild(movingCardIndex);

    private List<Transform> previousOrder = new List<Transform>();


    private void Awake()
    {
        var factionCards = GetComponentsInChildren<FactionDisplayCard>();

        int factionCount = ScenarioDataTypes._factionEnumValues.Length;

        totalCards = factionCards.Length;

        movingCardIndex = totalCards - 1;

        for (int i = 0; i < totalCards; i++)
        {
            // if we there are duplicate cards loop them accordingly
            factionCards[i].faction = ScenarioDataTypes._factionEnumValues[i % factionCount];
        }

        // Reverse order to get medieval first
        for (int i = 0; i < totalCards; i++)
        {
            transform.GetChild(0).SetSiblingIndex(totalCards - 1 - i);
        }

        cardWidth = factionCards[0].GetComponent<RectTransform>().rect.width;

        SnapCards();
    }

    private void Update()
    {
        HorizontalScrollHandler();
    }

    private void HorizontalScrollHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector2 mousePosWRTScrollArea;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                scrollArea,
                Input.mousePosition,
                null,
                out mousePosWRTScrollArea
            );

            if (!scrollArea.rect.Contains(mousePosWRTScrollArea))
                return;

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

            movingCard.localPosition += new Vector3(mainScroll, 0, 0);

            for (int i = 0; i < movingCardIndex; i++)
            {
                var cardScroll = dragDeltaX * cardScrollSpeed * i;

                transform.GetChild(i).localPosition += new Vector3(cardScroll, 0, 0);
            }

            if (isDragLeft)
            {
                // Auto Snap card when the position of the card near to the left side card
                if (Mathf.Abs(movingCard.localPosition.x) > cardWidth + lastCardOffset * 0.95f)
                {
                    movingCard.SetAsFirstSibling();
                    SnapCards();
                }
            }
            else if (isDragRight)
            {
                // this is to change moving card(which is always last child) when slide to the right
                if (movingCard.localPosition.x > 0.1f)
                {
                    transform.GetChild(0).SetAsLastSibling();
                }
                // if the user dragged enough to right set last visible card (which is always first child) to the left
                if (Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.2f)
                {
                    transform.GetChild(0).localPosition = new Vector3(-(cardWidth + lastCardOffset), 0, 0);
                }

                // Auto snap the cards when nearly close to the default position(0,0,0)
                if (Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.05f)
                {
                    SnapCards();
                }
            }

            wasDraggingLeft = totalDrag < 0;
            wasDraggingRight = totalDrag > 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            // Only snap if enough card position is dragged on mouse release
            if (wasDraggingRight && Mathf.Abs(movingCard.localPosition.x) < cardWidth * 0.6f)
            {
                SnapCards();
            }
            else if (wasDraggingLeft && Mathf.Abs(movingCard.localPosition.x) > cardWidth * 0.35f)
            {
                movingCard.SetAsFirstSibling();
                SnapCards();
            }
            else
            {
                // if enough card position is not dragged on mouse release reset the order
                ResetOrder();
            }
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

    public FactionName GetCurrentFaction()
    {
        return movingCard.GetComponent<FactionDisplayCard>().faction;
    }
}