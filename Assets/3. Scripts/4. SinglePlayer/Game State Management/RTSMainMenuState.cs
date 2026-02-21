using UnityEngine;

public class RTSMainMenuState : RTSState
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.MenuMusic);
    }

    public override void OnStateExit()
    {

    }
}
