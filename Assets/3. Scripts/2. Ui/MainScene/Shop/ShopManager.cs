using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    private void Start()
    {
        backBtn.onClick.AddListener(OnclickCloseBtn);
    }

    private void OnclickCloseBtn()
    {
        // HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}
