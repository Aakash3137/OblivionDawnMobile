using UnityEngine;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [SerializeField] private FactionScrollPanelManager fspManager;
    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        var faction = fspManager.GetCurrentFaction();

        GameData.playerFaction = faction;

        GameStateManager.Instance.ChangeState(GameStateEnum.LOADING);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
}