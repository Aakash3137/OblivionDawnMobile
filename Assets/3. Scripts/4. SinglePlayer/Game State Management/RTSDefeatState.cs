using UnityEngine;

public class RTSDefeatState : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    private Canvas _gameOverCanvas;

    public void OnStateEnter()
    {
        Time.timeScale = 0f;
        _gameOverCanvas = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity);
    }

    public void OnStateExit()
    {
        Time.timeScale = 1f;
        Destroy(_gameOverCanvas, 0.1f);
    }
}
