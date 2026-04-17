using UnityEngine;

public class MainMenuState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.PlayInLoop(GameAudioType.MenuMusic);
    }

    public override void OnStateExit()
    {

    }
}
