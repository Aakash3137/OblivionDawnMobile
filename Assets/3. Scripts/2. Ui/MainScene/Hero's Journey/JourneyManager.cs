using System;
using UnityEngine;
using UnityEngine.UI;

public class JourneyManager : MonoBehaviour
{
    [SerializeField] internal Button cross_button;

    public void Start()
    {
        cross_button.onClick.AddListener(OnClickShowHomePage);
    }

    public void OnClickShowHomePage()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}
