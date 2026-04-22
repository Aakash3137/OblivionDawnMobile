using UnityEngine;
using UnityEngine.UI;

public class BattlePanelManager : MonoBehaviour
{
    [SerializeField] private Button battleButton;
    [SerializeField] private Button rewardButton;

    private void Start()
    {
        battleButton.onClick.AddListener(OnClickBattle);
        rewardButton.onClick.AddListener(OnClickReward);
    }

    private void OnClickBattle()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var gameModeManager = GameModeManager.Instance;

        if (gameModeManager != null)
            gameModeManager.ShowPanel();
        else
            Debug.LogError("GameMode Manager is null");
    }
    private void OnClickReward()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var rewardPanel = RewardPanelScript.Instance;

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
