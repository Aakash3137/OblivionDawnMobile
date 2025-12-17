using Fusion;
using UnityEngine;

public class NetworkBuilding : NetworkBehaviour
{
    [Networked] public NetworkSide OwnerSide { get; set; }

    public GameObject playerVisual;
    public GameObject enemyVisual;

    public override void Spawned()
    {
        Debug.Log($"[NetworkBuilding] Spawned on {(Runner.IsServer ? "HOST" : "CLIENT")} | HasInputAuthority={Object.HasInputAuthority} | InputAuthority={Object.InputAuthority}");
        
        // Find player visual (first child) if not assigned
        if (playerVisual == null && transform.childCount > 0)
        {
            playerVisual = transform.GetChild(0).gameObject;
            Debug.Log($"[NetworkBuilding] Found playerVisual: {playerVisual.name}");
        }
        
        // Find enemy visual if not assigned (should be assigned in prefab or created by manager)
        if (enemyVisual == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains("Enemy"))
                {
                    enemyVisual = transform.GetChild(i).gameObject;
                    Debug.Log($"[NetworkBuilding] Found enemyVisual: {enemyVisual.name}");
                    break;
                }
            }
        }
        
        if (enemyVisual == null)
        {
            Debug.LogWarning($"[NetworkBuilding] No enemy visual found for {name}. Assign enemyVisual in prefab or ensure it's created as child.");
        }
        
        UpdateVisuals();
    }

    public override void FixedUpdateNetwork()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        bool isMyBuilding = Object.InputAuthority == Runner.LocalPlayer;

        if (playerVisual != null)
            playerVisual.SetActive(isMyBuilding);

        if (enemyVisual != null)
            enemyVisual.SetActive(!isMyBuilding);
    }
}
