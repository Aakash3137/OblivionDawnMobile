using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text subTitleText;
    private RTSGameStateManager gmInstance;


    private void Start()
    {
        gmInstance = RTSGameStateManager.Instance;

        menuButton.onClick.AddListener(HomeMenu);
        restartButton.onClick.AddListener(RestartGame);

        if (RTSGameStateManager.Instance.GetCurrentState() == RTSGameState.DEFEAT)
        {
            subTitleText.text = "Defeat";
        }
        else if (RTSGameStateManager.Instance.GetCurrentState() == RTSGameState.VICTORY)
        {
            subTitleText.text = "Victory";
        }
    }

    private void RestartGame()
    {
        gmInstance.ChangeState(RTSGameState.PLAYING);
        int currentSceneName = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneName);
    }

    private void HomeMenu()
    {
        gmInstance.ChangeState(RTSGameState.MAIN_MENU);
        SceneManager.LoadScene("MainScene");
    }
}
