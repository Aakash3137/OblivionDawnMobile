using UnityEngine;

public class UnitProgress : ProgressManager
{
    private UnitSpawnerScenario spawner;
    private float currentTime;

    private void Start()
    {
        spawner = GetComponentInParent<UnitSpawnerScenario>();
        currentTime = 0f;
    }

    private void Update()
    {
        ///
        /// DO NOT DELETE COMMENTS
        /// 

        // if (!spawner.HasResources())
        // {
        //     currentTime = 0f;
        //     progressAmount = 0f;
        //     UpdateFillAmount(progressAmount);
        //     return;
        // }

        if (currentTime > spawner.unitBuildTime)
        {
            currentTime = 0f;
        }

        currentTime += Time.deltaTime;
        progressAmount = currentTime / spawner.unitBuildTime;
        UpdateFillAmount(progressAmount);
    }
}
