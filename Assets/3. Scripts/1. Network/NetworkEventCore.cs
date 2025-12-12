using UnityEngine;
using Fusion;
using System;
using System.Collections.Generic;

public enum NetworkEventTargets {
    All,
    Others,
    HostOnly,
    ClientsOnly
}

public enum EventCode : ushort {
    PlayerJoined = 1,
    BuildRequest = 10, // client → host
    BuildSync = 11     // host → clients
}

public class NetworkEventCore : NetworkBehaviour
{
    public static NetworkEventCore Instance;
    public static PlayerRef LastEventSender { get; private set; }

    private static Dictionary<EventCode, Action<string>> eventHandlers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=cyan>[NetworkEventCore]</color> Initialized and persistent.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------- LISTENERS ----------------
    public static void AddListener(EventCode evt, Action<string> callback)
    {
        if (!eventHandlers.ContainsKey(evt)) eventHandlers.Add(evt, null);
        eventHandlers[evt] += callback;
        Debug.Log($"[NetworkEventCore] Listener added for {evt}");
    }

    public static void RemoveListener(EventCode evt, Action<string> callback)
    {
        if (eventHandlers.ContainsKey(evt)) eventHandlers[evt] -= callback;
    }

    // ---------------- RAISE EVENT ----------------
    public static void RaiseEvent<T>(EventCode evt, T data, NetworkEventTargets target) where T : struct
    {
        if (Instance == null)
        {
            Debug.LogError("[NetworkEventCore] No instance in scene!");
            return;
        }

        string json = JsonUtility.ToJson(data);
        Debug.Log($"[NetworkEventCore] RaiseEvent: {evt} | Target: {target} | Data: {json}");

        switch (target)
        {
            case NetworkEventTargets.All:
                Instance.RPC_All(evt, json);
                break;
            case NetworkEventTargets.HostOnly:
                Instance.RPC_Host(evt, json);
                break;
            case NetworkEventTargets.ClientsOnly:
                Instance.RPC_Clients(evt, json);
                break;
            case NetworkEventTargets.Others:
                Instance.RPC_Others(evt, json);
                break;
        }
    }

    // ---------------- RPCS ----------------
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_All(EventCode evt, string json) => ProcessEvent(evt, json);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Host(EventCode evt, string json, RpcInfo info = default)
    {
        if (Runner.IsServer)
        {
            LastEventSender = info.Source;
            ProcessEvent(evt, json);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Clients(EventCode evt, string json)
    {
        if (!Runner.IsServer) ProcessEvent(evt, json);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Others(EventCode evt, string json)
    {
        if (!Runner.IsServer) ProcessEvent(evt, json);
    }

    // ---------------- PROCESS EVENT ----------------
    private void ProcessEvent(EventCode evt, string json)
    {
        Debug.Log($"[NetworkEventCore] Event received: {evt} | Data: {json}");

        if (!eventHandlers.ContainsKey(evt) || eventHandlers[evt] == null)
        {
            Debug.LogWarning($"[NetworkEventCore] No listener for {evt}");
            return;
        }

        eventHandlers[evt]?.Invoke(json);
    }
}
