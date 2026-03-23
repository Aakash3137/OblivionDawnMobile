using UnityEngine;
using UnityEngine.UI;

public class OpenShopPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenPanel);
    }

    private void OpenPanel()
    {
        panel.SetActive(true);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OpenPanel);
    }
}
