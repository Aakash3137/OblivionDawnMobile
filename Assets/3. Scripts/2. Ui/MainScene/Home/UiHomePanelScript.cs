using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using System;
using Ricimi;

public class UiHomePanelScript : MonoBehaviour
{
    #region Variable and References
    [Header("User Data")]
    [SerializeField] private Userdata PlayerData;

    [Header("UI")]
    [SerializeField] private TMP_Text UserNameTxt;
    [SerializeField] private TMP_Text LevelNotTxt;
    [SerializeField] private TMP_Text CoinsTxt;
    [SerializeField] private TMP_Text DiamondsTxt;
    [SerializeField] private Image UserPic;

    [Header("Selection Window")]
    [SerializeField] internal GameObject SelectionWindown;

    [SerializeField] private float duration = 0.7f;

    #endregion

    #region  LifeCycle

    private void OnEnable()
    {
        UserNameTxt.text = PlayerData.UserName;
        if (PlayerData.ProfilePicture != null)
        {
            UserPic.sprite = PlayerData.ProfilePicture;
        }
        else
        {
            UserPic.sprite = PlayerData.defaultProfilePicture;
        }

        LevelNotTxt.text = PlayerData.Level.ToString();
        CoinsTxt.text = PlayerData.Coins.ToString();
        DiamondsTxt.text = PlayerData.Diamonds.ToString();
    }

    public void OpenProfileManager()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Profile);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    public void OnClickPlayButton()
    {
        SelectionWindown.SetActive(true);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    public void OnClickCloseSelectionWindow()
    {
        StartCoroutine(ZoomAndClose());
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    IEnumerator ZoomAndClose()
    {
        Vector3 startScale = SelectionWindown.transform.localScale;
        Vector3 endScale = Vector3.zero;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SelectionWindown.transform.localScale =
                Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }

        SelectionWindown.transform.localScale = startScale; // reset for next open
        SelectionWindown.SetActive(false);
    }
    #endregion
}
