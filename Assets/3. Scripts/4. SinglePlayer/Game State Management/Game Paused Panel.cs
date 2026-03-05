using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePausedPanel : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;

    // [SerializeField] private Button resumeButton;
    private GameStateManager gmInstance;


    private void Start()
    {
        gmInstance = GameStateManager.Instance;

        homeButton.onClick.AddListener(HomeMenu);
        restartButton.onClick.AddListener(RestartGame);
        // resumeButton.onClick.AddListener(ResumeGame);
    }

    private void ResumeGame()
    {
        gmInstance.ChangeState(GameState.PLAYING);
    }

    private void RestartGame()
    {
        gmInstance.ChangeState(GameState.PLAYING);
        int currentSceneName = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneName);
    }

    private void HomeMenu()
    {
        gmInstance.ChangeState(GameState.MAIN_MENU);
        SceneManager.LoadScene("MainScene");
    }
}
