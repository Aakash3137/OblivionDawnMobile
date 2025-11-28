using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] internal GameObject profilePanel;
    [SerializeField] internal GameObject HomePanel;
    [SerializeField] internal TMP_InputField usernameInputField;
    [SerializeField] internal TMP_Text saveText;
    [SerializeField] internal Button saveButton;
    [SerializeField] internal Button crossButton;

    private PlayerProfile playerProfile;

    private void Start()
    {
        playerProfile = PlayerProfile.LoadFromDisk();
        usernameInputField.text = playerProfile.PlayerName;

        saveButton.onClick.AddListener(SaveProfile);
        crossButton.onClick.AddListener(CloseProfileManager);
    }

    private void SaveProfile()
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

    private void CloseProfileManager()
    {
        profilePanel.SetActive(false);
        HomePanel.SetActive(true);
    }

    IEnumerator ClearSaveTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        saveText.text = "";
    }
}