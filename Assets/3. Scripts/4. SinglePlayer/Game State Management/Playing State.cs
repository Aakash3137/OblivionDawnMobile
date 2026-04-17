using UnityEngine;

public class PlayingState : GameState
{
    public override void OnStateEnter()
    {
        AudioManager.PlayInLoop(GameAudioType.GameMusic);
    }
    public override void OnStateExit()
    {

    }
}
