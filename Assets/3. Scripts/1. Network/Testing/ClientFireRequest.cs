using UnityEngine;
using System;

[Serializable]
public struct PlayerFireState2 {
    public int playerId;
    public bool startFire;
}
public class ClientFireRequest : MonoBehaviour {

    public void SendFireRequest(int myId, bool firing) {

        PlayerFireState2 fireData = new PlayerFireState2 {
            playerId = myId,
            startFire = firing
        };

        NetworkEventCore.RaiseEvent(
            EventCode.Custom,
            fireData,
            NetworkEventTargets.HostOnly
        );

        Debug.Log($"[Client → Host] Request → Player:{myId} Fire:{firing}");
    }
}