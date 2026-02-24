using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public TabsController tabsController;

    public void OnClick()
    {
        if (tabsController == null)
        {
            Debug.LogError("TabsController reference is missing!");
            return;
        }

        tabsController.SelectTab((int)UITab.Shop);
    }
    
}
