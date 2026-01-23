using UnityEngine;

public class VisualsComponent : MonoBehaviour
{
    public Visuals visuals;
    private IdentityComponent identityComponent;
    private Renderer visualsRenderer;

    public void Initialize(Visuals visuals)
    {
        this.visuals = visuals;

        AssignMaterials();
    }

    private void AssignMaterials()
    {
        if (visuals.playerUnitMaterial == null || visuals.enemyUnitMaterial == null)
        {
            Debug.Log($"<color=magenta> Failed to assign materials for {name}</color>");
        }

        identityComponent = TryGetComponent<IdentityComponent>(out var identity) ? identity : null;
        visualsRenderer = TryGetComponent<Renderer>(out var rendererComponent) ? rendererComponent : null;

        if (visualsRenderer != null)
        {
            switch (identityComponent.side)
            {
                case Side.Player:
                    visualsRenderer.sharedMaterial = visuals.playerUnitMaterial;
                    break;
                case Side.Enemy:
                    visualsRenderer.sharedMaterial = visuals.enemyUnitMaterial;
                    break;
            }
        }
    }
}
