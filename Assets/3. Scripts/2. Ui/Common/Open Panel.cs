using UnityEngine;
using UnityEngine.UI;

public class OpenPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickOpenPanel);
    }

    private void OnClickOpenPanel()
    {
        panel.SetActive(true);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickOpenPanel);
    }
}
