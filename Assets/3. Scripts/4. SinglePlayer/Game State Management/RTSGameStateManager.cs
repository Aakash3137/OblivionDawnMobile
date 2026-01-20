using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// ═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════
// Game States : menu → load map → play → pause → victory/defeat
// ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
[System.Serializable]
public enum RTSGameState
{
    BOOTING,           // Initial splash / init
    MAIN_MENU,         // Single/multiplayer buttons, settings
    LOADING_MAP,       // Async load scene/map
    LOBBY,             // Multiplayer: host/join/wait for players (optional)
    PLAYING,           // Core RTS: select units, build, attack, harvest
    PAUSED,            // ESC menu: resume/save/quit/options
    VICTORY,           // Win screen: stats, replay, next mission
    DEFEAT,            // Loss screen: retry/quit
    CUTSCENE           // Intro/outro videos (common in campaigns)
}

// ═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════
// Manager (singleton pattern)
// ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
public class RTSGameStateManager : MonoBehaviour
{
    public static RTSGameStateManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private RTSGameState currentState = RTSGameState.BOOTING;
    [SerializeField] private RTSGameState previousState;

    [Header("Callbacks (assign in Inspector or code)")]
    public UnityEvent<RTSGameState> onStateChanged;  // Fired every change
    public RTSStateCallbacks[] stateCallbacks;        // Per-state enter/exit

    private Dictionary<RTSGameState, UnityEvent> onEnterEvents = new();
    private Dictionary<RTSGameState, UnityEvent> onExitEvents = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
            RegisterDictionary();
            InitializeCallbacks();
            ChangeState(RTSGameState.PLAYING);  // Start here
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // void OnValidate()
    // {
    //     var enumValues = Enum.GetValues(typeof(RTSGameState));
    //     stateCallbacks = new RTSStateCallbacks[enumValues.Length];

    //     for (int i = 0; i < enumValues.Length; i++)
    //     {
    //         stateCallbacks[i] = new RTSStateCallbacks();
    //         stateCallbacks[i].state = (RTSGameState)enumValues.GetValue(i);
    //     }
    // }

    void InitializeCallbacks()
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
    void RegisterDictionary()
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

    public void ChangeState(RTSGameState newState)
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
        Debug.Log($"[RTSState] {previousState} → {currentState}");
    }

    // ───── Query Helpers ─────
    public bool Is(RTSGameState state) => currentState == state;
    public bool Was(RTSGameState state) => previousState == state;
    public bool InGameplay => currentState == RTSGameState.PLAYING || currentState == RTSGameState.PAUSED;

    // ───── RTS Shortcuts ─────
    public void TogglePause()
    {
        if (Is(RTSGameState.PLAYING)) ChangeState(RTSGameState.PAUSED);
        else if (Is(RTSGameState.PAUSED)) ChangeState(RTSGameState.PLAYING);
    }

    public void ToMainMenu() => ChangeState(RTSGameState.MAIN_MENU);
    public void StartGame() => ChangeState(RTSGameState.LOADING_MAP);
    public void GameWon() => ChangeState(RTSGameState.VICTORY);
    public void GameLost() => ChangeState(RTSGameState.DEFEAT);
}

// Inspector-friendly callback holder
[Serializable]
public class RTSStateCallbacks
{
    public RTSGameState state;
    public UnityEvent onEnter;
    public UnityEvent onExit;
}