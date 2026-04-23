using UnityEngine;
using UnityEngine.UI;

public class BattlePanelManager : MonoBehaviour
{
    [SerializeField] private Button battleButton;
    [SerializeField] private Button rewardButton;

    private OverlayPanelManager overlayPanelManager;

    private void Start()
    {
        battleButton.onClick.AddListener(OnClickBattle);
        rewardButton.onClick.AddListener(OnClickReward);

        overlayPanelManager = OverlayPanelManager.Instance;
    }

    private void OnClickBattle()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var gameModeManager = overlayPanelManager.gameModePanel;

        if (gameModeManager != null)
            gameModeManager.OpenGameModePanel();
        else
            Debug.LogError("GameMode Manager is null");
    }
    private void OnClickReward()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var rewardPanel = overlayPanelManager.rewardPanel;

        if (rewardPanel != null)
            rewardPanel.OpenRewardPanel();
        else
            Debug.LogError("Reward Panel is null");
    }

    private void OnDestroy()
    {
        battleButton.onClick.RemoveListener(OnClickBattle);
        rewardButton.onClick.RemoveListener(OnClickReward);
    }
}
