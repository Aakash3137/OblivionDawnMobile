using UnityEngine;

public enum GameModeType
{
    HostClient,  // Host creates room, client joins with code
    PvP         // Random matchmaking
}

public class CustomGameMode : MonoBehaviour
{
    public static GameModeType CurrentGameMode { get; private set; } = GameModeType.HostClient;
    public static bool IsMatchmakingGame => CurrentGameMode == GameModeType.PvP;
    
    public static CustomGameMode Instance { get; private set; }
    
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
    }
    
    public static void SetGameMode(GameModeType mode)
    {
        CurrentGameMode = mode;
        Debug.Log($"[CustomGameMode] Set to: {mode}");
    }
}