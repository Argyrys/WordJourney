using UnityEngine;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [Header("Ad Unit IDs - Replace with your own after installing AdMob SDK")]
    public string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

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
        Debug.Log("AdsManager: Google Mobile Ads SDK not yet integrated. Add real AdMob integration for production.");
    }

    public void LoadBannerAd()
    {
        Debug.Log("LoadBannerAd - AdMob not integrated yet");
    }

    public void ShowBannerAd()
    {
        Debug.Log("ShowBannerAd - AdMob not integrated yet");
    }

    public void HideBannerAd()
    {
        Debug.Log("HideBannerAd - AdMob not integrated yet");
    }

    public void LoadInterstitialAd()
    {
        Debug.Log("LoadInterstitialAd - AdMob not integrated yet");
    }

    public void ShowInterstitialAd()
    {
        Debug.Log("ShowInterstitialAd - AdMob not integrated yet");
    }

    public void LoadRewardedAd()
    {
        Debug.Log("LoadRewardedAd - AdMob not integrated yet");
    }

    public void ShowRewardedAd(Action callback)
    {
        Debug.Log("ShowRewardedAd - AdMob not integrated yet, granting reward as placeholder");
        callback?.Invoke();
    }

    public bool IsRewardedAdReady()
    {
        return false;
    }
}
