using UnityEngine;

public class DefeatState : GameState
{
    [SerializeField] private Canvas gameOverCanvas;
    private Canvas instantiatedCanvas;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;

        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(true);
        else
            instantiatedCanvas = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity);
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1f;

        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(false);
    }
}
