using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanelScript : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Userdata _UserData;

    [Header("UI_Button")]
    [SerializeField] private Button LoginBtn;


    [Header("UI_Text")]
    [SerializeField] private TMP_InputField Input_UserName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoginBtn.onClick.AddListener(OnClickLogin);
    }

    void OnClickLogin()
    {
        _UserData.UserName = Input_UserName.text;
        _UserData.GuestUser = true;
        Debug.Log("Login Succesfully With " + _UserData.UserName);
        // HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}
