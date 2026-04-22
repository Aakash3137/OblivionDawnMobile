using UnityEngine;
using UnityEngine.UI;

public class BattlePanelManager : MonoBehaviour
{
    [SerializeField] private Button battleButton;

    private void Start()
    {
        battleButton.onClick.AddListener(OnClickBattle);
    }

    private void OnClickBattle()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        GameModeManager.Instance.ShowPanel();
    }

    private void OnDestroy()
    {
        battleButton.onClick.RemoveListener(OnClickBattle);
    }

}
