using UnityEngine;
using UnityEngine.UI;

public class DeckCard : MonoBehaviour
{
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button deckDeSelectButton;
    [SerializeField] private GameObject selectionPanel;

    private UpgradeCard upgradeCard;
    private ScriptableObject upgradeDataSO => upgradeCard.upgradeDataSO;

    private void Awake()
    {
        upgradeCard = GetComponent<UpgradeCard>();
    }

    private void Start()
    {
        selectionPanel.SetActive(false);

        deckSelectButton.onClick.AddListener(OnClickDeckCard);
        deckDeSelectButton.onClick.AddListener(OnClickDeSelectCard);
    }

    private void OnClickDeckCard()
    {
        bool selected = DeckSelectionManager.Instance.TrySelectCard(upgradeDataSO);
        selectionPanel.SetActive(selected);
    }

    private void OnClickDeSelectCard()
    {
        bool deselected = DeckSelectionManager.Instance.TryDeSelectCard(upgradeDataSO);
        selectionPanel.SetActive(!deselected);
    }

    private void OnDestroy()
    {
        deckSelectButton.onClick.RemoveListener(OnClickDeckCard);
        deckDeSelectButton.onClick.RemoveListener(OnClickDeSelectCard);
    }
}