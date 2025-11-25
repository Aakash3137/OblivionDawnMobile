using System;
using UnityEngine;

[Serializable]
public struct PlayerFireState1 {
    public int playerId;
    public bool startFire;
}
public class HostFireSender : MonoBehaviour {

    public void HostStartsFire(int hostPlayerId) 
    {
        PlayerFireState1 fireData = new PlayerFireState1 {
            playerId = hostPlayerId,
            startFire = true
        };

        NetworkEventCore.RaiseEvent(
            EventCode.Custom,
            fireData,
            NetworkEventTargets.ClientsOnly
        );

        Debug.Log($"[Host → Clients] Fire → ID:{hostPlayerId}");
    }
}
