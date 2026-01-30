using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] internal GameObject profilePanel;

    [SerializeField] internal TMP_Text NameText;
    [SerializeField] private TMP_Text Coins;
    [SerializeField] private TMP_Text Gems;
    [SerializeField] internal Button crossButton;
    [SerializeField] internal Button EditButton;
    [SerializeField] internal Image UserPic, HomePic;

    [SerializeField] internal Userdata ProfileData;
    [SerializeField] internal ProfileEditData profileEditData;

    private void Start()
    {
        SetProfile();
    }

    private void SetProfile()
    {
        if (ProfileData.ProfilePicture != null)
        {
            UserPic.sprite = ProfileData.ProfilePicture; 
            HomePic.sprite = ProfileData.ProfilePicture;
            profileEditData.EditorPic.sprite = ProfileData.ProfilePicture;
        }
        else
        {
            UserPic.sprite = ProfileData.defaultProfilePicture;
            HomePic.sprite = ProfileData.defaultProfilePicture;
            profileEditData.EditorPic.sprite = ProfileData.defaultProfilePicture;
        }
        
        NameText.text = ProfileData.UserName;
        Coins.text = ProfileData.Coins.ToString();
        Gems.text = ProfileData.Diamonds.ToString();
        profileEditData.NameInputField.text = ProfileData.UserName;
        EditButton.onClick.AddListener(OpenEditProfilePanel);
        crossButton.onClick.AddListener(CloseProfileManager);
        profileEditData.CloseButton.onClick.AddListener(CloseEditProfilePanel);
        profileEditData.SaveButton.onClick.AddListener(SaveProfile);
        profileEditData.DeleteButton.onClick.AddListener(DeleteAccount);
        profileEditData.ConfirmDeleteButton.onClick.AddListener(OpenDeleteAlertPanel);
        profileEditData.Setdata(this);
    }
    private void CloseProfileManager()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }

    public void OpenEditProfilePanel()
    {
        profileEditData.ProfileEditPanel.gameObject.SetActive(true);
    }

    public void CloseEditProfilePanel()
    {
        profileEditData.ProfileEditPanel.gameObject.SetActive(false);
    }

    public void OpenDeleteAlertPanel()
    {
        profileEditData.DeleteAlertPanel.SetActive(true);
    }

    public void SaveProfile()
    {
        string newUsername = profileEditData.NameInputField.text;
        Debug.Log($"[ProfileManager] Saving Username: {newUsername}");
        if (!string.IsNullOrEmpty(newUsername))
        {
            PlayerPrefs.SetString("Username", newUsername);
            ProfileData.UserName = newUsername;
            NameText.text = newUsername;
            ProfileData.Birthday = profileEditData.dateDD.value.ToString("00") + "/" + profileEditData.monthDD.value.ToString("00");
            UserPic.sprite = profileEditData.TargetImage;
            HomePic.sprite = profileEditData.TargetImage;
            ProfileData.ProfilePicture = UserPic.sprite;

            StartCoroutine(ShowMessageThenClear(2f, "Profile Saved!", Color.green, profileEditData.MessageTxt));
        }
        else
        {
            StartCoroutine(ShowMessageThenClear(2f, "Username cannot be empty.", Color.red, profileEditData.MessageTxt));
        }
    }


    IEnumerator ShowMessageThenClear(float delay, string message, Color color, TMP_Text MessageObject)
    {
        MessageObject.text = message;
        MessageObject.color = color;
        MessageObject .gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        MessageObject.text = "";
        MessageObject.gameObject.SetActive(false);
        profileEditData.ProfileEditPanel.SetActive(false);
    }

    public void OnClickChangeProfilePic()
    {
        Debug.Log("[ProfileManager] Change Profile Picture Clicked");

    }


    public void DeleteAccount()
    {
        ProfileData.ResetData();
        PlayerPrefs.DeleteAll();
        profileEditData.DeleteAlertPanel.SetActive(false);
        HomeUIManager.Instance.ShowPanel(PanelName.Login);
    }

    
}

[System.Serializable]
public class ProfileEditData
{
    public TMP_InputField NameInputField;
    public string Birthday;
    public TMP_Text MessageTxt;
    

    public TMP_Dropdown dateDD;
    public TMP_Dropdown monthDD;
    public GameObject ProfileEditPanel;
    public GameObject DeleteAlertPanel;

    public Button CloseButton;
    public Button SaveButton;
    public Button DeleteButton;
    public Button ConfirmDeleteButton;

    [Header ("Avatar Selection")]
    public List<Sprite> AvatarOptions;
    public List<ImageSelector> CharacterImages;
    public ImageSelector AvatarPrefab;
    public Transform AvatarParent;
    public Sprite TargetImage;
    public Button OnSelectAvatarButton;
    public Button CharacterEditButton;
    public GameObject AvtarSelectionPanel;
    public Image EditorPic;

    public void Setdata(ProfileManager profileManager)
    {
        OnSelectAvatarButton.onClick.AddListener(OnSelectAvatar);
        GenerateMonthOptions();
        GenerateCharacterImages(profileManager);
        CharacterEditButton.onClick.AddListener(OnClickCharacterEditButton);
    }

    private void OnClickCharacterEditButton()
    {
        AvtarSelectionPanel.SetActive(true);
    }

    public void OnSelectAvatar()
    {
        Debug.Log("[ProfileEditData] Avatar Selected: " + TargetImage.name);
        EditorPic.sprite = TargetImage;
        AvtarSelectionPanel.SetActive(false);
    }


    public void GenerateMonthOptions()
    {
        monthDD.ClearOptions();
        List<string> monthOptions = new List<string>();

        for (int i = 1; i <= 12; i++)
        {
            monthOptions.Add(i.ToString("00"));
        }

        monthDD.AddOptions(monthOptions);

        monthDD.onValueChanged.AddListener(OnMonthChanged);

        OnMonthChanged(monthDD.value);
    }

    void OnMonthChanged(int monthIndex)
    {
        int month = monthIndex + 1; 
        GenerateDateOptions(month);
    }

    void GenerateDateOptions(int month)
    {
        dateDD.ClearOptions();
        List<string> dateOptions = new List<string>();

        int daysInMonth = GetDaysInMonth(month);

        for (int i = 1; i <= daysInMonth; i++)
        {
            dateOptions.Add(i.ToString("00"));
        }

        dateDD.AddOptions(dateOptions);
    }

    int GetDaysInMonth(int month)
    {
        switch (month)
        {
            case 1: return 31;
            case 2: return 28; 
            case 3: return 31;
            case 4: return 30;
            case 5: return 31;
            case 6: return 30;
            case 9: return 30;
            case 10: return 31;
            case 11: return 30;
            case 12: return 31;
            default:
                return 31;
        }
    }


    void GenerateCharacterImages(ProfileManager profileManager)
    {
        if(AvatarParent.childCount > 0)
        {
            for(int i = AvatarParent.childCount -1; i >=0; i--)
            {
                GameObject.Destroy(AvatarParent.GetChild(i).gameObject);
            }
        }   
        CharacterImages.Clear();

        foreach(Sprite CharSprite in AvatarOptions)
        {
            ImageSelector newAvatar = GameObject.Instantiate(AvatarPrefab, AvatarParent);
            newAvatar.name = "Avatar_" + CharSprite.name;
            newAvatar.CharacterImage.sprite = CharSprite;
            newAvatar.TargetImage = EditorPic;
            newAvatar.tem_Selected.gameObject.SetActive(false);
            newAvatar._ProfileManager = profileManager;
            CharacterImages.Add(newAvatar);

        }
    }

}
