using UnityEngine;
using UnityEngine.UI;

public class FactionButton : MonoBehaviour
{
    [SerializeField] private FactionScrollHandler fspManager;
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
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
    }
}