using UnityEngine;

public class LoadingState : GameState
{
    [SerializeField] private Canvas loadingPanelCanvas;
    private Canvas instantiatedCanvas;

    public override void OnStateEnter()
    {
        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(true);
        else
            instantiatedCanvas = Instantiate(loadingPanelCanvas, Vector3.zero, Quaternion.identity, transform);

    }

    public override void OnStateExit()
    {
        if (instantiatedCanvas != null)
            instantiatedCanvas.gameObject.SetActive(false);
    }
}
