using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayBlock : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] internal Button claimButton;
    [SerializeField] internal TMP_Text dayText;
    [SerializeField] internal TMP_Text rewardAmountText;
    [SerializeField] internal Image Background;
 
    [SerializeField] private Sprite BG_Claimed;
    [SerializeField] private Sprite BG_Unclaimed;
    [SerializeField] private Sprite BG_Locked;

    [Header("Overlays")]
    [SerializeField] internal GameObject claimedOverlay;
    [SerializeField] internal GameObject unclaimedOverlay;
    [SerializeField] internal GameObject lockedOverlay;
    [SerializeField] public GameObject Connector;

    [Header ("Popup References")]
    [SerializeField] internal GameObject rewardPopup;

    // ==============================
    // UNLOCKED (Claimable Today)
    // ==============================
    public void SetUnlocked()
    {
        Background.sprite = BG_Unclaimed;
        lockedOverlay.SetActive(false);
        claimedOverlay.SetActive(false);
        unclaimedOverlay.SetActive(false);
        claimButton.gameObject.SetActive(true);
    }

    // ==============================
    // LOCKED (Future Day)
    // ==============================
    public void SetLocked()
    {
        Background.sprite = BG_Locked;
        lockedOverlay.SetActive(true);
        claimedOverlay.SetActive(false);
        unclaimedOverlay.SetActive(false);
        claimButton.gameObject.SetActive(false);
    }

    // ==============================
    // CLAIMED (Already Collected)
    // ==============================
    public void SetClaimed()
    {
        Background.sprite = BG_Claimed;
        lockedOverlay.SetActive(false);
        claimedOverlay.SetActive(true);
        unclaimedOverlay.SetActive(false);
        claimButton.gameObject.SetActive(false);
    }

    public void SetDayTxt(int No)
    {
        dayText.text = "DAY " + (No+1).ToString();
    }
}