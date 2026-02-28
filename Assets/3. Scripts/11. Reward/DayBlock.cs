using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayBlock : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] internal Button claimButton;
    [SerializeField] internal TMP_Text dayText;
    [SerializeField] internal TMP_Text rewardAmountText;

    [Header("Overlays")]
    [SerializeField] internal GameObject claimedOverlay;
    [SerializeField] internal GameObject unclaimedOverlay;
    [SerializeField] internal GameObject lockedOverlay;

    // ==============================
    // UNLOCKED (Claimable Today)
    // ==============================
    public void SetUnlocked()
    {
        lockedOverlay.SetActive(false);
        claimedOverlay.SetActive(false);
        unclaimedOverlay.SetActive(true);
        claimButton.gameObject.SetActive(true);
    }

    // ==============================
    // LOCKED (Future Day)
    // ==============================
    public void SetLocked()
    {
        lockedOverlay.SetActive(true);
        claimedOverlay.SetActive(false);
        unclaimedOverlay.SetActive(true);
        claimButton.gameObject.SetActive(false);
    }

    // ==============================
    // CLAIMED (Already Collected)
    // ==============================
    public void SetClaimed()
    {
        lockedOverlay.SetActive(false);
        claimedOverlay.SetActive(true);
        unclaimedOverlay.SetActive(false);
        dayText.transform.parent.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);
    }
}