using UnityEngine;
using System;
using System.Collections;

public static class Generic
{
    private class DelayRunner : MonoBehaviour { }

    private static DelayRunner runner;

    private static DelayRunner Runner
    {
        get
        {
            if (runner != null)
                return runner;

            var go = new GameObject("[Generic.Delay]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            runner = go.AddComponent<DelayRunner>();
            return runner;
        }
    }

    public static void Delay(Action action, float time)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        #endif
        Runner.StartCoroutine(DelayCoroutine(action, time));
    }

    private static IEnumerator DelayCoroutine(Action action, float time)
    { 
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
