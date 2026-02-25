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
        if (!spawner.isProducingUnits)
        {
            currentTime = 0f;
            progressAmount = currentTime / waitTime;
            UpdateFillAmount(progressAmount);
            canvasGroup.alpha = 0f;
            return;
        }

        if (canvasGroup.alpha != 1f)
            canvasGroup.alpha = 1f;

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
            canvasGroup.alpha = 0f;
        }
    }

}
