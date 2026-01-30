using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public void TogglePanelFunc()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
