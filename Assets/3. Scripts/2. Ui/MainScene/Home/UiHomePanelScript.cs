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
    // [SerializeField] private TMP_Text UserNameTxt;
    [SerializeField] private TMP_Text LevelNotTxt;
    // [SerializeField] private TMP_Text CoinsTxt;
    [SerializeField] private TMP_Text DiamondsTxt;
    [SerializeField] private Image UserPic;

    // [Header("Selection Window")]
    // [SerializeField] internal GameObject SelectionWindown;

    // [SerializeField] private float duration = 0.7f;

    // public Buttonrefernce Buttons;

    #endregion

    #region  LifeCycle

    private void OnEnable()
    {
        // UserNameTxt.text = PlayerData.UserName;
        if (PlayerData.ProfilePicture != null)
        {
            UserPic.sprite = PlayerData.ProfilePicture;
        }
        else
        {
            UserPic.sprite = PlayerData.defaultProfilePicture;
        }

        LevelNotTxt.text = PlayerData.Level.ToString();
        // CoinsTxt.text = PlayerData.Coins.ToString();
        DiamondsTxt.text = PlayerData.Diamonds.ToString();
    }

    // void Start()
    // {
    //     Buttons.Play.onClick.AddListener(OnClickPlayButton);
    //     Buttons.Profile.onClick.AddListener(OpenProfileManager);
    //     Buttons.Shop.onClick.AddListener(OnClickShopButton);
    //     Buttons.Level.onClick.AddListener(OnClickLevelButton);
    //     Buttons.Setting.onClick.AddListener(OnClickSettingButton);
    //     Buttons.Deck.onClick.AddListener(OnClickDeckButton);
    //     Buttons.Upgrade.onClick.AddListener(OnClickUpgradeButton);
    //     Buttons.HeroJourney.onClick.AddListener(OnClickHeroJourney);

    // }


    // public void OnClickHomeButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Home);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // public void OpenProfileManager()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Profile);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }


    // public void OnClickPlayButton()
    // {
    //     SelectionWindown.SetActive(true);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }


    // public void OnClickShopButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Shop);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }


    // public void OnClickSettingButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Setting);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }


    // public void OnClickDeckButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Deck);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // public void OnClickUpgradeButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Upgrade);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // public void OnClickHeroJourney()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.HeroJourney);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // public void OnClickLevelButton()
    // {
    //     HomeUIManager.Instance.ShowPanel(PanelName.Level);
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // public void OnClickCloseSelectionWindow()
    // {
    //     StartCoroutine(ZoomAndClose());
    //     AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    // }

    // IEnumerator ZoomAndClose()
    // {
    //     Vector3 startScale = SelectionWindown.transform.localScale;
    //     Vector3 endScale = Vector3.zero;

    //     float t = 0f;
    //     while (t < duration)
    //     {
    //         t += Time.deltaTime;
    //         SelectionWindown.transform.localScale =
    //             Vector3.Lerp(startScale, endScale, t / duration);
    //         yield return null;
    //     }

    //     SelectionWindown.transform.localScale = startScale; // reset for next open
    //     SelectionWindown.SetActive(false);
    // }
    #endregion
}

// [System.Serializable]
// public class Buttonrefernce
// {
//     public Button Deck;
//     public Button Upgrade;
//     public Button HeroJourney;
//     public Button Level;
//     public Button Shop;
//     public Button Setting;
//     public Button Play;
//     public Button Profile;
// }
