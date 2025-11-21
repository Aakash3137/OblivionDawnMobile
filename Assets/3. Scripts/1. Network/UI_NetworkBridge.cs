using UnityEngine;

/// <summary>
/// Minimal bridge between network events and your HomeUIManager.
/// Keeps HomeUIManager free of network logic.
/// </summary>
public class UI_NetworkBridge : MonoBehaviour
{
    private void OnEnable()
    {
        PhotonNetworkManager.Instance.OnLobbyCreated += HandleLobbyCreated;
        PhotonNetworkManager.Instance.OnLobbyJoined += HandleLobbyJoined;

        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent += HandlePlayerJoined;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent += HandlePlayerLeft;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.OnLobbyCreated -= HandleLobbyCreated;
            PhotonNetworkManager.Instance.OnLobbyJoined -= HandleLobbyJoined;
        }

        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent -= HandlePlayerJoined;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent -= HandlePlayerLeft;
        }
    }

    private void HandleLobbyCreated(string code)
    {
        Debug.Log($"[UI Bridge] Lobby created: {code}");
        HomeUIManager.Instance.LoadingPanel.SetActive(false);
        HomeUIManager.Instance.PlayerJoinedPanel.SetActive(true);
        // Optionally show code text somewhere in your UI
    }

    private void HandleLobbyJoined()
    {
        Debug.Log("[UI Bridge] Joined Lobby");
        HomeUIManager.Instance.LoadingPanel.SetActive(false);
        HomeUIManager.Instance.PlayerJoinedPanel.SetActive(true);
    }

    private void HandlePlayerJoined(Fusion.PlayerRef p)
    {
        Debug.Log($"[UI Bridge] Player Joined: {p}");
        // Refresh your UI player list here (you can query NetworkPlayer objects)
    }

    private void HandlePlayerLeft(Fusion.PlayerRef p)
    {
        Debug.Log($"[UI Bridge] Player Left: {p}");
        // Update UI accordingly
    }
}
