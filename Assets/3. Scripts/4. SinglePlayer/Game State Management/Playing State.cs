using UnityEngine;

public class PlayingState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.TransitionAudio(GameAudioType.GameMusic);
    }
    public override void OnStateExit()
    {

    }
}
