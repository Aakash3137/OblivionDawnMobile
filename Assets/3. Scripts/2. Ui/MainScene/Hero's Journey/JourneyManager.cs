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

    void OnEnable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    private void OnDisable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    public void OnClickShowHomePage()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
}
