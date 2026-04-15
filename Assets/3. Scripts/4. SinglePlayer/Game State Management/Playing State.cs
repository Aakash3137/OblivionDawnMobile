using UnityEngine;

public class PlayingState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.Play(GameAudioType.GameMusic);
    }
    public override void OnStateExit()
    {

    }
}
