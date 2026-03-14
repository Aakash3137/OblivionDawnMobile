using System;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}

[Serializable]
public enum GameStateEnum
{
    BOOTING,
    MAIN_MENU,
    LOADING,
    LOBBY,
    PLAYING,
    PAUSED,
    VICTORY,
    DEFEAT,
    DRAW,
    CUTSCENE
}

[Serializable]
public class GameStateCallbacks
{
    public GameStateEnum state;
    public UnityEvent onEnter;
    public UnityEvent onExit;
}