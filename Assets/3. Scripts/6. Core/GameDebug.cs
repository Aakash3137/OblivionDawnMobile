using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class GameDebug
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static bool Enabled = true;
#endif

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!Enabled)
            return;
        Debug.Log(message);
        #endif
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(string message)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!Enabled)
            return;
        Debug.LogWarning(message);
        #endif
    }

    public static void Error(string message)
    {
        Debug.LogError(message);
    }

    public static void Error(string message, Object context)
    {
        Debug.LogError(message, context);
    }
    
}