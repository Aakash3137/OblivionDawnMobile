using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour
{
    public TextMeshProUGUI playerSlot1;
    public TextMeshProUGUI playerSlot2;

    private void OnEnable()
    {
        RefreshUI();
        PhotonEventsHandler.Instance.OnPlayerJoinedEvent += _ => RefreshUI();
        PhotonEventsHandler.Instance.OnPlayerLeftEvent += _ => RefreshUI();
    }

    private void OnDisable()
    {
        if (PhotonEventsHandler.Instance != null)
        {
            PhotonEventsHandler.Instance.OnPlayerJoinedEvent -= _ => RefreshUI();
            PhotonEventsHandler.Instance.OnPlayerLeftEvent -= _ => RefreshUI();
        }
    }

    private void RefreshUI()
    {
        var players = FindObjectsOfType<NetworkPlayer>().ToList();

        playerSlot1.text = players.Count > 0 ? players[0].PlayerName.ToString() : "Waiting...";
        playerSlot2.text = players.Count > 1 ? players[1].PlayerName.ToString() : "Waiting...";
    }
}