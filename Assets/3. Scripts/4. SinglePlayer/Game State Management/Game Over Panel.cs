using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text subTitleText;
    [SerializeField] private TMP_Text XP_Text;
    private GameStateManager gmInstance;
    [SerializeField] private LevelData LevelsData;


    private void Start()
    {
        gmInstance = GameStateManager.Instance;

        homeButton.onClick.AddListener(HomeMenu);
        restartButton.onClick.AddListener(RestartGame);
        //Calculate XP on gameover 
        XPCalculator.Instance.UpdateXP(GameStateManager.Instance.GetCurrentState());

        if (GameStateManager.Instance.GetCurrentState() == GameState.DEFEAT)
        {
            subTitleText.text = "Defeat";
        }
        else if (GameStateManager.Instance.GetCurrentState() == GameState.VICTORY)
        {
            subTitleText.text = "Victory";
        }
        LevelsData.SetXP(XPCalculator.Instance.total_XP);
        XP_Text.text = "XP + " + XPCalculator.Instance.total_XP;
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
