using UnityEngine;

public class PausedState : GameState
{
    [SerializeField] private Canvas gamePausedCanvas;
    private Canvas _gamePausedObject;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;

        if (_gamePausedObject != null)
            _gamePausedObject.gameObject.SetActive(true);
        else
            _gamePausedObject = Instantiate(gamePausedCanvas, Vector3.zero, Quaternion.identity);
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1f;

        if (_gamePausedObject != null)
            _gamePausedObject.gameObject.SetActive(false);
    }
}
