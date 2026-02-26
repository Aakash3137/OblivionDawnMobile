using UnityEngine;

public class VictoryState : State
{
    [SerializeField] private Canvas gameOverCanvas;
    private Canvas _gameOverObject;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;

        if (_gameOverObject != null)
            _gameOverObject.gameObject.SetActive(true);
        else
            _gameOverObject = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity);
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1f;
        _gameOverObject.gameObject.SetActive(false);
    }
}
