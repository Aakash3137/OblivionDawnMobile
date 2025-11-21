using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private readonly List<PlayerRef> _players = new();

    [Header("Game Scene (Fusion SceneRef)")]
    public SceneRef GameScene;

    public float CountdownTime = 5f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent += HandlePlayerJoined;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent += HandlePlayerLeft;
        }
    }


    private void OnDisable()
    {
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent -= HandlePlayerJoined;
            PhotonEventsHandler.Instance.OnPlayerLeftEvent -= HandlePlayerLeft;
        }
    }

    private void HandlePlayerJoined(PlayerRef player)
    {
        if (!_players.Contains(player))
            _players.Add(player);

        var runner = PhotonNetworkManager.Instance.Runner;

        if (_players.Count == 2 && runner.IsServer)
        {
            StartCoroutine(StartCountdown());
        }
    }

    private void HandlePlayerLeft(PlayerRef player)
    {
        _players.Remove(player);
    }
    
    private IEnumerator StartCountdown()
    {
        Debug.Log($"[Lobby] Match starts in {CountdownTime} seconds...");
        yield return new WaitForSeconds(CountdownTime);

        var runner = PhotonNetworkManager.Instance.Runner;

        if (runner != null && runner.IsServer)
        {
            Debug.Log("[Lobby] Loading game scene...");

            runner.SceneManager.LoadScene(GameScene, default);
        }
    }

}