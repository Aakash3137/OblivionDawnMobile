using UnityEngine;

public class RTSDefeatState : RTSState
{
    [SerializeField] private Canvas gameOverCanvas;
    private Canvas _gameOverCanvas;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;
        _gameOverCanvas = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity);
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1f;
        Destroy(_gameOverCanvas, 0.1f);
    }
}
