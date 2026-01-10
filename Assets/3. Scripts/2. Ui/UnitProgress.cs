using UnityEngine;

public class UnitProgress : ProgressManager
{
    private UnitSpawnerScenario unitSpawnStats;
    private float currentTime;

    private void Start()
    {
        unitSpawnStats = GetComponentInParent<UnitSpawnerScenario>();
        currentTime = 0f;
    }

    private void Update()
    {
        if (currentTime > unitSpawnStats.unitBuildTime)
        {
            currentTime = 0f;
        }

        currentTime += Time.deltaTime;
        progressAmount = currentTime / unitSpawnStats.unitBuildTime;
        UpdateFillAmount(progressAmount);
    }
}
