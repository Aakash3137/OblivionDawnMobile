using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] internal GameObject profilePanel;
    [SerializeField] internal TMP_InputField usernameInputField;
    [SerializeField] internal TMP_Text saveText;
    [SerializeField] internal Button saveButton;    
    [SerializeField] internal Button crossButton;
    [SerializeField] internal Button renameButton;

    private PlayerProfile playerProfile;

    [SerializeField] internal Userdata ProfileData;

    private void Start()
    {
        playerProfile = PlayerProfile.LoadFromDisk();
        usernameInputField.text = playerProfile.PlayerName;

        crossButton.onClick.AddListener(CloseProfileManager);
        renameButton.onClick.AddListener(RenameProfileName);
        saveButton.onClick.AddListener(SaveProfileName);
    }

    //Edit profile on UI Profile Panel itself
    private void RenameProfileName()
    {
        usernameInputField.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);
    }
    private void SaveProfileName()
    {
        SaveProfile();
        usernameInputField.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
    }
     private void CloseProfileManager()
    {
        profilePanel.SetActive(false);
    }

    public void SaveProfile()
    {
        string newUsername = usernameInputField.text;
        Debug.Log($"[ProfileManager] Saving Username: {newUsername}");
        if (!string.IsNullOrEmpty(newUsername))
        {
            PlayerPrefs.SetString("Username", newUsername);
            saveText.text = "Profile Saved!";
        }
        else
        {
            saveText.text = "Username cannot be empty.";
        }
        StartCoroutine(ClearSaveTextAfterDelay(2f));
    }

    IEnumerator ClearSaveTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        saveText.text = "";
    }
}