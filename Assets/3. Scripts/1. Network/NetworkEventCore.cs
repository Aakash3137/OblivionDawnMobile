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
    PlayerShot = 2,
    Custom = 3
}

public class NetworkEventCore : NetworkBehaviour {

    public static NetworkEventCore Instance;

    private static Dictionary<EventCode, Action<string>> eventHandlers 
        = new Dictionary<EventCode, Action<string>>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            Debug.Log("<color=cyan>[NetworkEventCore]</color> Initialized.");
        }
    }

    // ---------------- LISTENERS ----------------

    public static void AddListener(EventCode evt, Action<string> callback) {
        if (!eventHandlers.ContainsKey(evt))
            eventHandlers.Add(evt, callback);

        eventHandlers[evt] += callback;
        Debug.Log($"[NetworkEventCore] Listener added for {evt}");
    }

    public static void RemoveListener(EventCode evt, Action<string> callback) {
        if (eventHandlers.ContainsKey(evt))
            eventHandlers[evt] -= callback;
    }

    // ---------------- SEND EVENT ----------------

    public static void RaiseEvent<T>(EventCode evt, T data, NetworkEventTargets target) where T : struct {

        if (Instance == null) {
            Debug.LogError("[NetworkEventCore] ERROR: No instance found in scene!");
            return;
        }

        string json = JsonUtility.ToJson(data);

        Debug.Log($"<color=yellow>[NetworkEventCore]</color> Sending Event: {evt} | Target: {target} | Data: {json}");

        switch (target) {
            case NetworkEventTargets.All:
                Instance.RPC_Event(evt, json);
                break;

            case NetworkEventTargets.Others:
                Instance.RPC_Event_Others(evt, json);
                break;

            case NetworkEventTargets.HostOnly:
                Instance.RPC_Event_Host(evt, json);
                break;

            case NetworkEventTargets.ClientsOnly:
                Instance.RPC_Event_Clients(evt, json);
                break;
        }
    }

    // ---------------- RPC ROUTING ----------------

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Event(EventCode evt, string json) => ProcessEvent(evt, json);

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Event_Others(EventCode evt, string json) { 
        if (!Runner.IsServer) ProcessEvent(evt, json); 
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Event_Host(EventCode evt, string json) { 
        if (Runner.IsServer) ProcessEvent(evt, json); 
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Event_Clients(EventCode evt, string json) {
        if (!Runner.IsServer) ProcessEvent(evt, json);
    }

    // ---------------- PROCESS ----------------

    private void ProcessEvent(EventCode evt, string json) {

        Debug.Log($"<color=green>[NetworkEventCore]</color> Received Event: <b>{evt}</b> | Data: {json}");

        if (!eventHandlers.ContainsKey(evt)) {
            Debug.LogWarning($"[NetworkEventCore] No listener found for event {evt}");
            return;
        }

        eventHandlers[evt]?.Invoke(json);
    }
}
