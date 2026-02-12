using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelNavigation : MonoBehaviour
{
    [Header("Faction Buttons : 0 = Medieval ; 1 = Present ; 2 = Future ; 3 = Galvadore")]
    [SerializeField] private List<Button> factionButtons;


    private void Start()
    {
        foreach (Button button in factionButtons)
        {
            // button.onClick.AddListener(() => UpgradePanelManager.Instance.SwitchFaction(button.transform.GetSiblingIndex()));
        }
    }


}
