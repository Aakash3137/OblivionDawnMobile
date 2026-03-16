using UnityEngine;

public class MainMenuState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.MenuMusic);
    }

    public override void OnStateExit()
    {

    }
}
