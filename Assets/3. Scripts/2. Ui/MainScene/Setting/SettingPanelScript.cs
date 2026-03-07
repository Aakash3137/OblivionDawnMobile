using UnityEngine;
using UnityEngine.UI;

public class SettingPanelScript : MonoBehaviour
{
    [Header ("Buttons")]
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button AudioBtn;
    [SerializeField] private Button VibrationBtn;
    [SerializeField] private Button MusicBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseButton.onClick.AddListener(OnClickClose);
    }

    public void OnClickClose()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}
