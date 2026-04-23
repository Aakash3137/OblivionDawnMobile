using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private GameStateEnum currentState = GameStateEnum.BOOTING;
    [SerializeField] private GameStateEnum previousState;

    [Header("Callbacks (assign in Inspector or code)")]
    public UnityEvent<GameStateEnum> onStateChanged;  // Fired every change
    public GameStateCallbacks[] stateCallbacks;

    private Dictionary<GameStateEnum, UnityEvent> onEnterEvents = new();
    private Dictionary<GameStateEnum, UnityEvent> onExitEvents = new();

    public bool IsGameOver = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        RegisterDictionary();
        InitializeCallbacks();
    }

    private void Start()
    {
        onEnterEvents[currentState]?.Invoke();
    }

    private void InitializeCallbacks()
    {
        foreach (var cb in stateCallbacks)
        {
            if (onEnterEvents.ContainsKey(cb.state))
                onEnterEvents[cb.state] = cb.onEnter;
            else
                onEnterEvents[cb.state] = new UnityEvent();

            if (onExitEvents.ContainsKey(cb.state))
                onExitEvents[cb.state] = cb.onExit;
            else
                onExitEvents[cb.state] = new UnityEvent();
        }
    }
    private void RegisterDictionary()
    {
        foreach (var cb in stateCallbacks)
        {
            if (cb.onEnter == null)
                cb.onEnter = new UnityEvent();

            if (cb.onExit == null)
                cb.onExit = new UnityEvent();

            onEnterEvents[cb.state] = cb.onEnter;
            onExitEvents[cb.state] = cb.onExit;
        }
    }

    public void ChangeState(GameStateEnum newState)
    {
        if (newState == currentState) return;

        // Exit old
        if (onExitEvents.TryGetValue(currentState, out var exitEvent))
            exitEvent?.Invoke();

        previousState = currentState;
        currentState = newState;

        // Enter new
        if (onEnterEvents.TryGetValue(currentState, out var enterEvent))
            enterEvent?.Invoke();

        onStateChanged?.Invoke(newState);
        Debug.Log($"[State] {previousState} → {currentState}");
    }

    public bool Is(GameStateEnum state) => currentState == state;
    public bool Was(GameStateEnum state) => previousState == state;
    public bool InGameplay => currentState == GameStateEnum.PLAYING || currentState == GameStateEnum.PAUSED;

    public void TogglePause()
    {
        if (Is(GameStateEnum.PLAYING)) ChangeState(GameStateEnum.PAUSED);
        else if (Is(GameStateEnum.PAUSED)) ChangeState(GameStateEnum.PLAYING);
    }

    public GameStateEnum GetCurrentState()
    {
        return currentState;
    }
}


