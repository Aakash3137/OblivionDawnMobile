using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

public class GoogleAdMobHandler : MonoBehaviour
{
    public static GoogleAdMobHandler Instance;

    [Header("Ad Unit IDs")]
    // [SerializeField] private string bannerAdUnit = "ca-app-pub-1914635053297075/2147037178";
    // [SerializeField] private string interstitialAdUnit = "ca-app-pub-1914635053297075/4390057135";
    // [SerializeField] private string rewardedAdUnit = "ca-app-pub-1914635053297075/1037953727";
    // [SerializeField] private string rewardedInterstitialAdUnit = "ca-app-pub-1914635053297075/9235189707";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // MobileAds.Initialize(initStatus => { LoadAllAds(); });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            LoadAd();
            // LoadInterstitialAd();
            LoadRewardedAd();
            // LoadRewardedInterstitialAd();
        });
    }


    #region Banner
    
    #if UNITY_ANDROID
        private string _adBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string _adBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string _adBannerUnitId = "unused";
    #endif
    BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adBannerUnitId, AdSize.Banner, AdPosition.Top);
    }

    void DestroyAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion

//     #region Interstitial Ads

//     // These ad units are configured to always serve test ads.
//     // private string _adInterstitialUnitId;
// #if UNITY_ANDROID
//     private string _adInterstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
// #elif UNITY_IPHONE
//      private string _adInterstitialUnitId = "ca-app-pub-3940256099942544/4411468910";
// #else
//     private string _adInterstitialUnitId = "unused";
// #endif

//     private InterstitialAd _interstitialAd;

//     /// <summary>
//     /// Loads the interstitial ad.
//     /// </summary>
//     public void LoadInterstitialAd()
//     {
//         // Clean up the old ad before loading a new one.
//         if (_interstitialAd != null)
//         {
//             _interstitialAd.Destroy();
//             _interstitialAd = null;
//         }

//         Debug.Log("Loading the interstitial ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();

//         // send the request to load the ad.
//         InterstitialAd.Load(_adInterstitialUnitId, adRequest,
//             (InterstitialAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("interstitial ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 Debug.Log("Interstitial ad loaded with response : "
//                           + ad.GetResponseInfo());

//                 _interstitialAd = ad;
//             });
//     }

//     /// <summary>
//     /// Shows the interstitial ad.
//     /// </summary>
//     public void ShowInterstitialAd()
//     {
//         if (_interstitialAd != null && _interstitialAd.CanShowAd())
//         {
//             Debug.Log("Showing interstitial ad.");
//             _interstitialAd.Show();
//         }
//         else
//         {
//             Debug.LogError("Interstitial ad is not ready yet.");
//         }
//     }

//     private void RegisterEventHandlers(InterstitialAd interstitialAd)
//     {
//         // Raised when the ad is estimated to have earned money.
//         interstitialAd.OnAdPaid += (AdValue adValue) =>
//         {
//             Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
//                 adValue.Value,
//                 adValue.CurrencyCode));
//         };
//         // Raised when an impression is recorded for an ad.
//         interstitialAd.OnAdImpressionRecorded += () =>
//         {
//             Debug.Log("Interstitial ad recorded an impression.");
//         };
//         // Raised when a click is recorded for an ad.
//         interstitialAd.OnAdClicked += () =>
//         {
//             Debug.Log("Interstitial ad was clicked.");
//         };
//         // Raised when an ad opened full screen content.
//         interstitialAd.OnAdFullScreenContentOpened += () =>
//         {
//             Debug.Log("Interstitial ad full screen content opened.");
//         };
//         // Raised when the ad closed full screen content.
//         interstitialAd.OnAdFullScreenContentClosed += () =>
//         {
//             Debug.Log("Interstitial ad full screen content closed.");
//         };
//         // Raised when the ad failed to open full screen content.
//         interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
//         {
//             Debug.LogError("Interstitial ad failed to open full screen content " +
//                         "with error : " + error);
//         };
//     }

//     private void RegisterReloadHandler(InterstitialAd interstitialAd)
//     {
//         // Raised when the ad closed full screen content.
//         interstitialAd.OnAdFullScreenContentClosed += () =>
//         {
//             Debug.Log("Interstitial Ad full screen content closed.");

//             // Reload the ad so that we can show another as soon as possible.
//             LoadInterstitialAd();
//         };
//         // Raised when the ad failed to open full screen content.
//         interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
//         {
//             Debug.LogError("Interstitial ad failed to open full screen content " +
//                         "with error : " + error);

//             // Reload the ad so that we can show another as soon as possible.
//             LoadInterstitialAd();
//         };
//     }

//     #endregion

    #region Rewarded Ad

    // These ad units are configured to always serve test ads.
    #if UNITY_ANDROID
        private string _adRewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
    #elif UNITY_IPHONE
        private string _adRewardedUnitId = "ca-app-pub-3940256099942544/1712485313";
    #else
        private string _adRewardedUnitId = "unused";
    #endif

    private RewardedAd _rewardedAd;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adRewardedUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                // 🔽 Register events after loading the ad
                RegisterEventHandlers(_rewardedAd);
                // RegisterReloadHandler(_rewardedAd);
            });


    }

    public void ShowRewardedAd()
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        Debug.Log("Show rewarded ad " + _rewardedAd);
        Debug.Log("Rewarded ad " +" Can Show " + _rewardedAd.CanShowAd());

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                 Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    // send the request to load the ad..

    private void RegisterEventHandlers(RewardedAd ad)
    {
        Debug.Log("Registering events for rewarded ad " + ad);
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        // ad.OnAdImpressionRecorded += () =>
        // {
        //     Debug.Log("Rewarded ad recorded an impression.");
        // };
        // // Raised when a click is recorded for an ad.
        // ad.OnAdClicked += () =>
        // {
        //     Debug.Log("Rewarded ad was clicked.");
        // };
        // // Raised when an ad opened full screen content.
        // ad.OnAdFullScreenContentOpened += () =>
        // {
        //     Debug.Log("Rewarded ad full screen content opened.");
        // };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
            //InvetoryManager._InventoryInstance.OnBuy();

        };
        // Raised when the ad failed to open full screen content.
        // ad.OnAdFullScreenContentFailed += (AdError error) =>
        // {
        //     Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
        // };
    }


    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    #endregion

//     #region Interstital Rewarded


//     // These ad units are configured to always serve test ads.
// #if UNITY_ANDROID
//     private string _adInterstitalRewardedUnitId = "ca-app-pub-3940256099942544/5354046379";
// #elif UNITY_IPHONE
//   private string _adInterstitalRewardedUnitId = "ca-app-pub-3940256099942544/6978759866";
// #else
//   private string _adInterstitalRewardedUnitId = "unused";
// #endif

//     private RewardedInterstitialAd _rewardedInterstitialAd;

//     /// <summary>
//     /// Loads the rewarded interstitial ad.
//     /// </summary>
//     public void LoadRewardedInterstitialAd()
//     {
//         // Clean up the old ad before loading a new one.
//         if (_rewardedInterstitialAd != null)
//         {
//             _rewardedInterstitialAd.Destroy();
//             _rewardedInterstitialAd = null;
//         }

//         Debug.Log("Loading the rewarded interstitial ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();
//         adRequest.Keywords.Add("unity-admob-sample");

//         // send the request to load the ad.
//         RewardedInterstitialAd.Load(_adInterstitalRewardedUnitId, adRequest,
//             (RewardedInterstitialAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("rewarded interstitial ad failed to load an ad " + "with error : " + error);
//                     return;
//                 }

//                 Debug.Log("Rewarded interstitial ad loaded with response : "
//                           + ad.GetResponseInfo());

//                 _rewardedInterstitialAd = ad;
//             });
//     }

//     public void ShowRewardedInterstitialAd()
//     {
//         const string rewardMsg =
//             "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

//         if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
//         {
//             rewardedInterstitialAd.Show((Reward reward) =>
//             {
//                 // TODO: Reward the user.
//                 Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
//             });
//         }
//     }

//     private void RegisterEventHandlers(RewardedInterstitialAd ad)
//     {
//         // Raised when the ad is estimated to have earned money.
//         ad.OnAdPaid += (AdValue adValue) =>
//         {
//             Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
//         };
//         // Raised when an impression is recorded for an ad.
//         ad.OnAdImpressionRecorded += () =>
//         {
//             Debug.Log("Rewarded interstitial ad recorded an impression.");
//         };
//         // Raised when a click is recorded for an ad.
//         ad.OnAdClicked += () =>
//         {
//             Debug.Log("Rewarded interstitial ad was clicked.");
//         };
//         // Raised when an ad opened full screen content.
//         ad.OnAdFullScreenContentOpened += () =>
//         {
//             Debug.Log("Rewarded interstitial ad full screen content opened.");
//         };
//         // Raised when the ad closed full screen content.
//         ad.OnAdFullScreenContentClosed += () =>
//         {
//             Debug.Log("Rewarded interstitial ad full screen content closed.");
//         };
//         // Raised when the ad failed to open full screen content.
//         ad.OnAdFullScreenContentFailed += (AdError error) =>
//         {
//             Debug.LogError("Rewarded interstitial ad failed to open " + "full screen content with error : " + error);
//         };
//     }
    
//     private void RegisterReloadHandler(RewardedInterstitialAd ad)
//     {
//         // Raised when the ad closed full screen content.
//         ad.OnAdFullScreenContentClosed += ()=>
//         {
//             Debug.Log("Rewarded interstitial ad full screen content closed.");

//             // Reload the ad so that we can show another as soon as possible.
//             LoadRewardedInterstitialAd();
//         };
//         // Raised when the ad failed to open full screen content.
//         ad.OnAdFullScreenContentFailed += (AdError error) =>
//         {
//             Debug.LogError("Rewarded interstitial ad failed to open " +
//                         "full screen content with error : " + error);

//             // Reload the ad so that we can show another as soon as possible.
//             LoadRewardedInterstitialAd();
//         };
//     }
//     #endregion




}
