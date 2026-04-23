using UnityEngine;
using UnityEngine.UI;

public class SettingPanelScript : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button AudioBtn;
    [SerializeField] private Button VibrationBtn;
    [SerializeField] private Button MusicBtn;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        CloseButton.onClick.AddListener(OnClickClose);
    }

    public void OnClickClose()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    public void OpenSettingPanel()
    {
        ShowPanel();
    }
    private void ShowPanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void HidePanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
