using UnityEngine;

public class MainMenuState : State
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.MenuMusic);
    }

    public override void OnStateExit()
    {

    }
}
