using UnityEngine;

public class FactionPanelScript : MonoBehaviour
{
    public CanvasGroup Layout;

    

    public void OnClickShowHomePage()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
    
}
