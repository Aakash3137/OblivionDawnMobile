using UnityEngine;

public class RTSPlayingState : RTSState
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.GameMusic);
    }
    public override void OnStateExit()
    {

    }
}
