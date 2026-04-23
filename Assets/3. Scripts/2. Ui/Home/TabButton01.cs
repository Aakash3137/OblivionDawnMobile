// using UnityEngine;
// using UnityEngine.UI;

// public class TabButton01 : MonoBehaviour
// {
//     [Header("UI References")]
//     public Image background;
//     public Image icon;
//     public GameObject highlight;

//     [Header("Sprites")]
//     public Sprite normalBg;
//     public Sprite selectedBg;

//     public Sprite normalIcon;
//     public Sprite selectedIcon;

//     [HideInInspector]
//     public int tabIndex;

//     private TabsController controller;

//     public void Init(TabsController tabsController, int index)
//     {
//         controller = tabsController;
//         tabIndex = index;
//     }

//     public void OnClick()
//     {
//         controller.SelectTab(tabIndex);
//     }

//     public void SetSelected(bool isSelected)
//     {
//         background.sprite = isSelected ? selectedBg : normalBg;
//         icon.sprite = isSelected ? selectedIcon : normalIcon;
//         highlight.SetActive(isSelected);
//     }
// }