using Fusion;
using UnityEngine;

public enum NetworkSide : int { None = 0, Player = 1, Enemy = 2 }

[RequireComponent(typeof(NetworkObject))]
public class NetworkTile : NetworkBehaviour
{
    [Header("Ownership (Set in Inspector BEFORE runtime)")]
    public NetworkSide ownerSide;    // Initial design-time owner (Player / Enemy)

    [Header("Visuals")]
    public Renderer tileRenderer;

    [Networked] public int OwnerInt { get; set; }

    private int _lastOwnerInt = -999;
    private NetworkSideManager _sideManager;

    public override void Spawned()
    {
        base.Spawned();

        // Find manager on all clients
        _sideManager = FindAnyObjectByType<NetworkSideManager>();

        // Register tile on the grid
        if (NetworkHexGridManager.Instance != null)
        {
            var coord = NetworkHexGridManager.Instance.WorldToHex(transform.position);
            NetworkHexGridManager.Instance.RegisterHex(coord, gameObject);
        }

        // Set initial ownership only on state authority (host/server)
        if (Object.HasStateAuthority)
        {
            OwnerInt = (int)ownerSide;
        }

        // Apply initial visuals
        _lastOwnerInt = OwnerInt;
        ApplyOwnerVisual((NetworkSide)OwnerInt);
    }

    public override void FixedUpdateNetwork()
    {
        // Detect replicated owner changes (Fusion 2 recommended pattern)
        if (_lastOwnerInt != OwnerInt)
        {
            _lastOwnerInt = OwnerInt;
            ApplyOwnerVisual((NetworkSide)OwnerInt);

            NetworkHexGridManager.Instance?.NotifyOwnerChanged();
        }
    }

    private void ApplyOwnerVisual(NetworkSide owner)
    {
        if (_sideManager == null)
            _sideManager = FindAnyObjectByType<NetworkSideManager>();

        if (_sideManager == null || tileRenderer == null)
            return;

        _sideManager.SetSide(tileRenderer, owner);

    }

    // ---------------------------------------------------------
    // NETWORK OWNERSHIP CONTROL
    // ---------------------------------------------------------

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSetOwner(int wantedOwner, RpcInfo info = default)
    {
        OwnerInt = wantedOwner;
    }

    public void RequestSetOwnerFromLocalClient(NetworkSide side)
    {
        if (!Object.HasInputAuthority) return;
        RPC_RequestSetOwner((int)side);
    }

    public void SetOwnerFromState(NetworkSide side)
    {
        if (!Object.HasStateAuthority) return;
        OwnerInt = (int)side;
    }
}
