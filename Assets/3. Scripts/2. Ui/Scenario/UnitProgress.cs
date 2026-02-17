using UnityEngine;

public class UnitProgress : ProgressManager
{
    private OffenseBuildingStats spawner;
    private float currentTime;

    float waitTime;

    private void Start()
    {
        spawner = GetComponentInParent<OffenseBuildingStats>();
        currentTime = 0f;

        waitTime = spawner.GetUnitSpawnTime();

        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        CheckBuildingSide();
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

        if (!spawner.isProducing && spawner.producedUnit != null)
        {
            currentTime = 0f;
            progressAmount = currentTime / waitTime;
            UpdateFillAmount(progressAmount);
            return;
        }

        if (currentTime > waitTime)
        {
            currentTime = 0f;
        }

        currentTime += Time.deltaTime;
        progressAmount = currentTime / waitTime;
        UpdateFillAmount(progressAmount);
    }

    //Disable UI for enemy
    private void CheckBuildingSide()
    {
        if (spawner.side == Side.Enemy)
        {
            _canvasGroup.alpha = 0f;
        }
    }

}
