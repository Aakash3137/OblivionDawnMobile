using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.BOOTING;
    [SerializeField] private GameState previousState;

    [Header("Callbacks (assign in Inspector or code)")]
    public UnityEvent<GameState> onStateChanged;  // Fired every change
    public GameStateCallbacks[] stateCallbacks;        // Per-state enter/exit

    private Dictionary<GameState, UnityEvent> onEnterEvents = new();
    private Dictionary<GameState, UnityEvent> onExitEvents = new();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
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

    public void ChangeState(GameState newState)
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

    public bool Is(GameState state) => currentState == state;
    public bool Was(GameState state) => previousState == state;
    public bool InGameplay => currentState == GameState.PLAYING || currentState == GameState.PAUSED;

    public void TogglePause()
    {
        if (Is(GameState.PLAYING)) ChangeState(GameState.PAUSED);
        else if (Is(GameState.PAUSED)) ChangeState(GameState.PLAYING);
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
}


