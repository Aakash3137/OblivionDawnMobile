using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelHandler : MonoBehaviour
{
    public static TopPanelHandler Instance { get; private set; }

    [SerializeField] private Userdata userdata;
    [SerializeField] private WeeklyRewardSO rewardSO;

    [Header("Text field references")]
    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private TMP_Text mapShardText;
    [SerializeField] private TMP_Text playerLevelText;

    [Header("Button references")]
    [SerializeField] private Button shardBuyButton;
    [SerializeField] private Button diamondBuyButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button heroJourneyButton;
    [SerializeField] private Button profileButton;

    [Header("Image references")]
    [SerializeField] private Image userPic;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        AddListeners();

        UpdateGemText(userdata.Diamonds);
        UpdateMapShardText(userdata.MapShards);
        UpdatePlayerLevelText(userdata.PlayerLevel);
    }

    private void OnClickProfileButton()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        throw new NotImplementedException();
    }

    private void OnClickHeroJourneyButton()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var heroJourneyPanel = HeroJourneyPanelManager.Instance;

        if (heroJourneyPanel != null)
            heroJourneyPanel.OpenJourneyPanel();
        else
            Debug.LogError("Hero Journey Panel Manager Handler is null");
    }

    private void OnClickSettingButton()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        throw new NotImplementedException();
    }

    private void OnClickBuyButton()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);

        var centerScrollPanel = CenterScrollHandler.Instance;

        if (centerScrollPanel != null)
            centerScrollPanel.SetPanel((int)MainPanels.Shop);
        else
            Debug.LogError("Center Scroll Handler is null");
    }
    private void UpdateGemText(int value)
    {
        diamondText.SetText("{0}", value);
    }
    private void UpdateMapShardText(int value)
    {
        mapShardText.SetText("{0}", value);
    }
    private void UpdatePlayerLevelText(int value)
    {
        playerLevelText.SetText("{0}", value);
    }

    private void AddListeners()
    {
        shardBuyButton.onClick.AddListener(OnClickBuyButton);
        diamondBuyButton.onClick.AddListener(OnClickBuyButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        heroJourneyButton.onClick.AddListener(OnClickHeroJourneyButton);
        profileButton.onClick.AddListener(OnClickProfileButton);

        userdata.OnDiamondsChanged += UpdateGemText;
        userdata.OnMapShardsChanged += UpdateMapShardText;
        userdata.OnPlayerLevelChanged += UpdatePlayerLevelText;
    }
    private void RemoveListeners()
    {
        shardBuyButton.onClick.RemoveListener(OnClickBuyButton);
        diamondBuyButton.onClick.RemoveListener(OnClickBuyButton);
        settingButton.onClick.RemoveListener(OnClickSettingButton);
        heroJourneyButton.onClick.RemoveListener(OnClickHeroJourneyButton);
        profileButton.onClick.RemoveListener(OnClickProfileButton);
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}
