using UnityEngine;
using UnityEngine.UI;
using System;

public enum MainPanels { Shop = 0, Level = 1, Battle = 2, Upgrades = 3, Deck = 4 }

public class CenterScrollHandler : MonoBehaviour
{
    public static CenterScrollHandler Instance { get; private set; }

    [SerializeField] private RectTransform viewport;
    [SerializeField] private ScrollRect scrollRect;
    [Header("Shop = 0, Level = 1, Battle = 2, Upgrades = 3, Deck = 4 ")]
    [SerializeField] private RectTransform[] menuPanels;

    [SerializeField, Space(10)] private float snapForce = 100f;
    [SerializeField, Space(10)] private float scrollThresholdVelocity = 200f;
    [field: SerializeField] public MainPanels defaultPanel { get; private set; } = MainPanels.Battle;

    public event Action<int> OnPanelChanged;

    public MainPanels currentPanel { get; private set; }
    private RectTransform contentTransform;
    private float width;
    private float snapSpeed;
    private bool isSnapped;
    private int previousIndex = -1;
    private int targetIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetScrollContentSize()
    {
        width = viewport.rect.width;

        for (int i = 0; i < menuPanels.Length; i++)
            menuPanels[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        if (TryGetComponent(out contentTransform))
            contentTransform.anchoredPosition = new Vector2(-width * (int)defaultPanel, contentTransform.anchoredPosition.y);
    }

    public void SetPanel(int index)
    {
        targetIndex = index;
        isSnapped = false;
        snapSpeed = 0f;
        scrollRect.velocity = Vector2.zero;
    }

    private void Update()
    {
        if (contentTransform == null) return;

        int currentIndex = Mathf.RoundToInt(-contentTransform.anchoredPosition.x / width);
        currentIndex = Mathf.Clamp(currentIndex, 0, menuPanels.Length - 1);
        currentPanel = (MainPanels)currentIndex;

        if (isSnapped && currentIndex != previousIndex)
        {
            previousIndex = currentIndex;
            OnPanelChanged?.Invoke(currentIndex);
        }

        int snapTarget = targetIndex >= 0 ? targetIndex : currentIndex;

        if (scrollRect.velocity.magnitude < scrollThresholdVelocity && !isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;

            float targetX = -width * snapTarget;
            contentTransform.anchoredPosition = new Vector2(
                Mathf.MoveTowards(contentTransform.anchoredPosition.x, targetX, snapSpeed),
                contentTransform.anchoredPosition.y);

            if (Mathf.Approximately(contentTransform.anchoredPosition.x, targetX))
            {
                contentTransform.anchoredPosition = new Vector2(targetX, contentTransform.anchoredPosition.y);
                isSnapped = true;
                targetIndex = -1;
            }
        }

        if (scrollRect.velocity.magnitude > scrollThresholdVelocity)
        {
            isSnapped = false;
            snapSpeed = 0f;
            targetIndex = -1;
        }
    }
}