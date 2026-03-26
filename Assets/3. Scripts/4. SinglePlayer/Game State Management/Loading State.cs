using UnityEngine;

public class LoadingState : GameState
{
    [SerializeField] private Canvas loadingPanelCanvas;

    private Canvas instantiatedCanvas;
    private LoadingPanelManager loadingPanelManager;

    public override void OnStateEnter()
    {
        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(true);
        else
        {
            instantiatedCanvas = Instantiate(loadingPanelCanvas, Vector3.zero, Quaternion.identity, transform);
            loadingPanelManager = instantiatedCanvas.GetComponentInChildren<LoadingPanelManager>();
        }

        loadingPanelManager.Initialize(GameData.gameMode);
    }

    public override void OnStateExit()
    {
        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(false);
    }
}
