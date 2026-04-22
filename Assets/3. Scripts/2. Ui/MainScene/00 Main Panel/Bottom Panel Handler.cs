using UnityEngine;
using UnityEngine.UI;

public class BottomPanelHandler : MonoBehaviour
{
    [Header("Shop = 0, Level = 1, Battle = 2, Upgrades = 3, Deck = 4 ")]
    [SerializeField] private Toggle[] toggles;

    private void Start()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(isOn => { if (isOn) TogglePanel(index); });
        }

        var defaultPanel = (int)CenterScrollHandler.Instance.defaultPanel;
        toggles[defaultPanel].SetIsOnWithoutNotify(true);

        CenterScrollHandler.Instance.OnPanelChanged += OnPanelChanged;
    }

    private void OnPanelChanged(int index)
    {
        for (int i = 0; i < toggles.Length; i++)
            toggles[i].SetIsOnWithoutNotify(i == index);
    }

    public void TogglePanel(int index)
    {
        CenterScrollHandler.Instance.SetPanel(index);
    }

    private void OnDestroy()
    {
        if (CenterScrollHandler.Instance != null)
            CenterScrollHandler.Instance.OnPanelChanged -= OnPanelChanged;
    }
}