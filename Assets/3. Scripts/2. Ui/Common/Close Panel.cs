using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickClosePanel);
    }

    private void OnClickClosePanel()
    {
        transform.parent.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickClosePanel);
    }
}
