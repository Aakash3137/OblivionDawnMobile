using System;
using UnityEngine;
using UnityEngine.Events;

public class State : MonoBehaviour
{
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}

[Serializable]
public enum GameState
{
    BOOTING,
    MAIN_MENU,
    LOADING_MAP,
    LOBBY,
    PLAYING,
    PAUSED,
    VICTORY,
    DEFEAT,
    CUTSCENE
}

[Serializable]
public class GameStateCallbacks
{
    public GameState state;
    public UnityEvent onEnter;
    public UnityEvent onExit;
}