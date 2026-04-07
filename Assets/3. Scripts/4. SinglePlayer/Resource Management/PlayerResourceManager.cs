using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerResourceManager : ResourceManager
{
    public static PlayerResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region  ContextMenu
    [Button]
    public void ResetResources()
    {
        startingResources[0].resourceAmount = 100;
        startingResources[1].resourceAmount = 100;
        startingResources[2].resourceAmount = 100;
        startingResources[3].resourceAmount = 100;

        SetResources(startingResources);
    }

    [Button]
    public void HackResources()
    {
        startingResources[0].resourceAmount = 999;
        startingResources[1].resourceAmount = 999;
        startingResources[2].resourceAmount = 999;
        startingResources[3].resourceAmount = 999;

        SetResources(startingResources);
    }

    public void Updateresources(float Value, BuildCost[] costs)
    {
        Debug.Log("Update Resources");
        for(int i = 0; i < 4; i++) 
        {
            int AMOUNT =  (int)(Value * costs[i].resourceAmount / 100f);
            Debug.Log("Amount Added " + AMOUNT);
            startingResources[i].resourceAmount -= AMOUNT;
        }
        SetResources(startingResources);
    }
    #endregion
}
