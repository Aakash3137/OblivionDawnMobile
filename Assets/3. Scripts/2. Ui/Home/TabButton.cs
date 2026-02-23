using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject highlight;

    [SerializeField] private Sprite normalBg;
    [SerializeField] private Sprite selectedBg;

    [SerializeField] private Sprite normalIcon;
    [SerializeField] private Sprite selectedIcon;

    private TabsController controller;
    private UITab tab;

    public void Initialize(TabsController owner, UITab tabType)
    {
        controller = owner;
        tab = tabType;
    }

    public void OnClick()
    {
        controller.SelectTab(tab);
    }

    public void SetSelected(bool selected)
    {
        background.sprite = selected ? selectedBg : normalBg;
        icon.sprite = selected ? selectedIcon : normalIcon;

        if (highlight != null)
            highlight.SetActive(selected);
    }
}