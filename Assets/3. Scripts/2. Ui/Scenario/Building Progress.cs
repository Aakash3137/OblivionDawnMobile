using System.Collections;
using UnityEngine;

public class BuildingProgress : ProgressManager
{
    private BuildingSkeleton buildingSkeleton;
    private float currentTime;
    float waitTime;

    public void Initialize()
    {
        buildingSkeleton = GetComponentInParent<BuildingSkeleton>();
        currentTime = 0f;
        waitTime = buildingSkeleton.buildTime;

        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        CheckBuildingSide();

        StartCoroutine(BuildingInProgress());
    }

    IEnumerator BuildingInProgress()
    {
        while (currentTime <= waitTime)
        {
            currentTime += Time.deltaTime;
            progressAmount = currentTime / waitTime;
            UpdateFillAmount(progressAmount);
            yield return null;
        }

        buildingSkeleton.OnBuildingCompleted();
        canvasGroup.alpha = 0f;
    }
    //Disable UI for enemy
    private void CheckBuildingSide()
    {
        if (buildingSkeleton.side == Side.Enemy)
        {
            canvasGroup.alpha = 0f;
        }
    }
}
