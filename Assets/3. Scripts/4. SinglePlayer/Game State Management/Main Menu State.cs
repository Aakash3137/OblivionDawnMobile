using UnityEngine;

public class MainMenuState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.Play(GameAudioType.MenuMusic);
    }

    public override void OnStateExit()
    {

    }
}
