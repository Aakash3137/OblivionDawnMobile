using UnityEngine;
using UnityEngine.UI;

public class CloseShopPanel : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        transform.parent.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(ClosePanel);
    }
}
