/*using Fusion;
using UnityEngine;

public class NetworkCardManager : MonoBehaviour
{
    public static NetworkCardManager Instance;
    
    [Header("References")]
    public CardData[] availableCards;
    
    private NetworkTile _selectedTile;
    private CardData _selectedCard;
    private NetworkRunner _runner;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
    }
    
    public void SelectCard(CardData card)
    {
        _selectedCard = card;
        Debug.Log($"[CardManager] Card selected: {card.cardName}");
    }
    
    public void OnTileClicked(NetworkTile tile)
    {
        if (_selectedCard == null) return;
        if (_runner == null || !_runner.IsRunning) return;
        
        _selectedTile = tile;
        tile.RPC_RequestSpawnCard(_selectedCard.cardName);
    }
    
    public void SpawnCardOnTile(NetworkTile tile, CardData card, PlayerRef owner, NetworkRunner runner)
    {
        if (card == null || card.prefab == null) return;
        
        Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
        
        if (card.cardType == CardType.Building || card.cardType == CardType.Resource)
        {
            var obj = runner.Spawn(card.prefab, spawnPos, Quaternion.identity, owner);
            var building = obj.GetComponent<NetworkBuilding>();
            if (building != null)
            {
                building.SetTile(tile);
            }
        }
        
        Debug.Log($"[CardManager] Spawned {card.cardName} on tile {tile.Object.Id}");
    }
    
    public CardData GetCardByName(string cardName)
    {
        return System.Array.Find(availableCards, c => c.cardName == cardName);
    }
}*/