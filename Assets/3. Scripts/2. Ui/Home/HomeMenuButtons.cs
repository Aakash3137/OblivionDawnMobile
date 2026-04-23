// using UnityEngine;
// using System.Collections.Generic;

// public class HomeMenuButtons : MonoBehaviour
// {
//     [SerializeField] private GameObject UILoginPanel;
//     [SerializeField] private GameObject ProfilePanel;
//     [SerializeField] private GameObject LoadingPanel;
//     [SerializeField] private GameObject GameModeSelectionPanel;
//     [SerializeField] private GameObject SettingsPanel;
//     [SerializeField] private GameObject HeroJourneyPanel;
//     [SerializeField] private GameObject PlayerFaction;

//     private List<GameObject> allPanels;

//     private void Awake()
//     {
//         // Add all panels to list
//         allPanels = new List<GameObject>
//         {
//             UILoginPanel,
//             ProfilePanel,
//             LoadingPanel,
//             GameModeSelectionPanel,
//             SettingsPanel,
//             HeroJourneyPanel
//         };
//     }

//     private void ShowPanel(GameObject panelToShow)
//     {
//         foreach (GameObject panel in allPanels)
//         {
//             panel.SetActive(panel == panelToShow);
//         }
//     }

//     public void OnClickUILoginPanel()
//     {
//         ShowPanel(UILoginPanel);
//     }


//     public void OnClickProfilePanel()
//     {
//         ShowPanel(ProfilePanel);
//     }

//     public void OnClickSettingsPanel()
//     {
//         ShowPanel(SettingsPanel);
//     }

//     public void OnClickLoadingPanel()
//     {
//         ShowPanel(LoadingPanel);
//     }

//     public void OnClickGameModeSelectionPanel()
//     {
//         ShowPanel(GameModeSelectionPanel);
//     }

//     public void OnClickHeroJourneyPanel()
//     {
//         ShowPanel(HeroJourneyPanel);
//     }

//     public void ClosePanel()
//     {
//         foreach (GameObject panel in allPanels)
//         {
//             panel.SetActive(false);
//         }
//     }

// }