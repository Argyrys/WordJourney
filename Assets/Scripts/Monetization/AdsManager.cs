using UnityEngine;
using System;

#if ADMOB_INSTALLED
using GoogleMobileAds.Api;
#endif

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [Header("Ad Unit IDs - Replace with your own")]
    public string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

#if ADMOB_INSTALLED
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
#endif

    private Action onRewardEarned;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAds();
    }

    private void InitializeAds()
    {
#if ADMOB_INSTALLED
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob SDK initialized");
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        });
#else
        Debug.Log("AdMob SDK not installed. Install Google Mobile Ads and add ADMOB_INSTALLED to Scripting Define Symbols.");
#endif
    }

    #region Banner Ads

    public void LoadBannerAd()
    {
#if ADMOB_INSTALLED
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
#endif
    }

    public void ShowBannerAd()
    {
#if ADMOB_INSTALLED
        if (bannerView != null)
        {
            bannerView.Show();
        }
#endif
    }

    public void HideBannerAd()
    {
#if ADMOB_INSTALLED
        if (bannerView != null)
        {
            bannerView.Hide();
        }
#endif
    }

    #endregion

    #region Interstitial Ads

    public void LoadInterstitialAd()
    {
#if ADMOB_INSTALLED
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        AdRequest request = new AdRequest();
        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            interstitialAd = ad;
            RegisterInterstitialEvents(ad);
        });
#endif
    }

    public void ShowInterstitialAd()
    {
#if ADMOB_INSTALLED
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd();
        }
#endif
    }

#if ADMOB_INSTALLED
    private void RegisterInterstitialEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to show: " + error);
            LoadInterstitialAd();
        };
    }
#endif

    #endregion

    #region Rewarded Ads

    public void LoadRewardedAd()
    {
#if ADMOB_INSTALLED
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }

        AdRequest request = new AdRequest();
        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                Invoke(nameof(LoadRewardedAd), 10f);
                return;
            }

            rewardedAd = ad;
            RegisterRewardedEvents(ad);
        });
#endif
    }

    public void ShowRewardedAd(Action callback)
    {
#if ADMOB_INSTALLED
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            onRewardEarned = callback;
            rewardedAd.Show((Reward reward) =>
            {
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready");
            LoadRewardedAd();
        }
#else
        Debug.Log("Rewarded ad placeholder - SDK not installed");
        callback?.Invoke();
#endif
    }

#if ADMOB_INSTALLED
    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            LoadRewardedAd();
        };
    }
#endif

    #endregion

    public bool IsRewardedAdReady()
    {
#if ADMOB_INSTALLED
        return rewardedAd != null && rewardedAd.CanShowAd();
#else
        return false;
#endif
    }

    private void OnDestroy()
    {
#if ADMOB_INSTALLED
        bannerView?.Destroy();
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
#endif
    }
}
