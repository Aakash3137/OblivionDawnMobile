using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;
    private RTSGameStateManager gmInstance;


    private void Start()
    {
        gmInstance = RTSGameStateManager.Instance;

        menuButton.onClick.AddListener(HomeMenu);
        restartButton.onClick.AddListener(RestartGame);
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
