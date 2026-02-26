using UnityEngine;

public class PlayingState : State
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.GameMusic);
    }
    public override void OnStateExit()
    {

    }
}
