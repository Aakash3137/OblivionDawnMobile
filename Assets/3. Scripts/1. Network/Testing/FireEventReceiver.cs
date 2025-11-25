using UnityEngine;

public class FireEventReceiver : MonoBehaviour {

    void OnEnable() {
        NetworkEventCore.AddListener(EventCode.Custom, OnFireEvent);
    }

    void OnDisable() {
        NetworkEventCore.RemoveListener(EventCode.Custom, OnFireEvent);
    }

    void OnFireEvent(string json) {

        PlayerFireState data = JsonUtility.FromJson<PlayerFireState>(json);

        Debug.Log($"🔥 Fire Event → Player:{data.playerId} | Start Fire:{data.startFire}");

        if (data.startFire)
            StartShootingAction(data.playerId);
        else
            StopShootingAction(data.playerId);
    }

    void StartShootingAction(int id) {
        Debug.Log($" Player {id} STARTED firing.");
        // TODO: apply shooting logic
    }

    void StopShootingAction(int id) {
        Debug.Log($" Player {id} STOPPED firing.");
        // TODO: stop shooting logic
    }
}