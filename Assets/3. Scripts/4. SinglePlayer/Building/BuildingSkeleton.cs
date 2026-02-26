using System;
using UnityEngine;

public class BuildingSkeleton : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] GenericComponents;
    [field: SerializeField] public float buildTime { get; private set; }
    public Side side { get; private set; }

    [SerializeField] ProgressManager progress;
    [SerializeField] BuildingProgress buildingProgress;
    public GameObject graphicObject { get; private set; }
    private GameObject constructionIcon;

    private async Awaitable Awake()
    {
        graphicObject = transform.GetChild(0).gameObject;
        graphicObject.SetActive(false);

        progress = GetComponentInChildren<UnitProgress>();
        if (progress == null)
        {
            progress = GetComponentInChildren<ResourceProgress>();
        }

        if (progress != null)
            progress.gameObject.SetActive(false);

        if (constructionIcon != null)
            constructionIcon.SetActive(true);

        await Awaitable.NextFrameAsync();

        GenericComponents = GetComponents<MonoBehaviour>();

        foreach (var component in GenericComponents)
        {
            if (component is BuildingPlacementHelper)
                continue;

            if (component != this)
                component.enabled = false;

            if (component is BuildingStats buildingStats)
            {
                buildTime = buildingStats.buildTime;
                side = buildingStats.side;
            }
        }

        buildingProgress = GetComponentInChildren<BuildingProgress>();
        buildingProgress.Initialize();
    }

    public void OnBuildingCompleted()
    {
        foreach (var component in GenericComponents)
        {
            if (!component.enabled)
                component.enabled = true;
        }

        if (progress != null && (progress is UnitProgress || progress is ResourceProgress))
        {
            progress.gameObject.SetActive(true);
        }

        if (constructionIcon != null)
            constructionIcon.SetActive(false);

        graphicObject.SetActive(true);
    }
}
