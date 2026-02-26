using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text subTitleText;
    private GameStateManager gmInstance;


    private void Start()
    {
        gmInstance = GameStateManager.Instance;

        homeButton.onClick.AddListener(HomeMenu);
        restartButton.onClick.AddListener(RestartGame);

        if (GameStateManager.Instance.GetCurrentState() == GameState.DEFEAT)
        {
            subTitleText.text = "Defeat";
        }
        else if (GameStateManager.Instance.GetCurrentState() == GameState.VICTORY)
        {
            subTitleText.text = "Victory";
        }
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
