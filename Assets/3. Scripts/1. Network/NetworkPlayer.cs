using Fusion;
using UnityEngine;

/// <summary>
/// Networked player info. Spawns once per player and syncs name + rank.
/// </summary>
public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public int Rank { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            // Only owner assigns name & rank.
            // If user has no saved name → auto random.
            string savedName = PlayerPrefs.GetString("PLAYER_NAME", "");

            if (string.IsNullOrWhiteSpace(savedName))
            {
                savedName = RandomNameGenerator.GetRandomName();
                PlayerPrefs.SetString("PLAYER_NAME", savedName);
            }

            PlayerName = savedName;

            // Rank fallback if not set
            Rank = PlayerPrefs.GetInt("PLAYER_RANK", Random.Range(1, 5));
        }

        Debug.Log($"[NetworkPlayer] Spawned | InputAuthority={Object.HasInputAuthority} | Name={PlayerName} | Rank={Rank}");
    }
}