using UnityEngine;
using System;

[Serializable]
public struct PlayerFireState {
    public int playerId;
    public bool startFire;
}

public class SharedModeFireSender : MonoBehaviour {

    public void FireButtonPressed(bool isFiring, int localPlayerId) {

        PlayerFireState fireData = new PlayerFireState {
            playerId = localPlayerId,
            startFire = isFiring
        };

        // Send to everyone
        NetworkEventCore.RaiseEvent(
            EventCode.Custom,
            fireData,
            NetworkEventTargets.All
        );

        Debug.Log($"[Shared Fire] Sent → Player:{fireData.playerId} Fire:{fireData.startFire}");
    }
}