using UnityEngine;

public class DefeatState : GameState
{
    [SerializeField] private Canvas gameOverCanvas;
    private Canvas instantiatedCanvas;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;
        GameStateManager.Instance.IsGameOver = true;
        if (GoogleAdMobHandler.Instance != null)
        {
            GoogleAdMobHandler.Instance.ShowRewardedAd();
        }

        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(true);
        else
            instantiatedCanvas = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity);
    }

    public void OnClickSkip()
    {
        
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1f;

        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(false);
    }
}
