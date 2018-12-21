using UnityEngine;
using System;
using System.Collections;
#if ENABLE_ADMOB
using GoogleMobileAds.Api;

public class AdMob : MonoBehaviour
{
    static private int coinsReward;
    static RewardBasedVideoAd rewardBasedVideo;
    static InterstitialAd interstitial;
    static string adAppId;
    static string adUnitId;
    static string adRewardedId;
    static bool isRewardedWatched = false;
    static bool isInterstitialReady = false;


    // Use this for initialization
    public void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
#if UNITY_ANDROID
        adAppId = DataManager.Instance.adsSettings.adMobAndroidAppID;
        adUnitId = DataManager.Instance.adsSettings.adMobAndroidInterstitialID;
        adRewardedId = DataManager.Instance.adsSettings.adMobAndroidRewardedID;

#elif UNITY_IOS
        adAppId = DataManager.Instance.adsSettings.adMobIosAppID;
        adRewardedId = DataManager.Instance.adsSettings.adMobIosRewardedID;
        adUnitId = DataManager.Instance.adsSettings.adMobIosInterstitialID;
#else
        adAppId = "unexpected_platform";
#endif
        Debug.LogWarning("INITIALIZING" + "\nAppID " + adAppId + "\nInterstitialID " + adUnitId + "\nRewardedID " + adRewardedId);
        MobileAds.Initialize(adAppId);
        coinsReward = DataManager.Instance.adsSettings.coinsAdReward;
        rewardBasedVideo = RewardBasedVideoAd.Instance;
        interstitial = new InterstitialAd(adUnitId);

        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded; //Subscribe method for event that is triggered when AD successfully watched
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        rewardBasedVideo.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideo.OnAdLoaded += HandleAdLoad;

        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitial.OnAdClosed += HandleOnAdClosed;
        interstitial.OnAdLoaded += HandleAdLoad;

        RequestAtStart();
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        if (sender.GetType() == typeof(RewardBasedVideoAd))
        {
            Debug.LogError("REWARDED AD FAILED TO LOAD \n" + e.Message);
        }
        else Debug.LogError("INTERSTITIAL AD FAILED TO LOAD \n" + e.Message);

    }

    static private void RequestRewardBasedVideo()
    {
        Debug.Log("Rewarded requested");
        // Create an empty ad request.
        AdRequest request;
        if (DataManager.Instance.adsSettings.nonpersonalizedAds)
        {
            Debug.LogWarning("NONPERSONALIZED REWARDED REQUESTED");
            request = new AdRequest.Builder().AddExtra("npa", "1").Build();
        }
        else request= new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        rewardBasedVideo.LoadAd(request, adRewardedId);
    }

    private static void RequestInterstitialVideo()
    {
#if UNITY_IOS
        interstitial.OnAdFailedToLoad -= HandleOnAdClosed;
        interstitial.OnAdClosed -= HandleOnAdClosed;
        interstitial.OnAdLoaded -= HandleAdLoad;
        interstitial = new InterstitialAd(adUnitId);
        interstitial.OnAdFailedToLoad += HandleOnAdClosed;
        interstitial.OnAdClosed += HandleOnAdClosed;
        interstitial.OnAdLoaded += HandleAdLoad;
#endif
        Debug.Log("Interst requested");
        AdRequest request;
        if (DataManager.Instance.adsSettings.nonpersonalizedAds)
        {
            Debug.LogWarning("NONPERSONALIZED AD REQUESTED");
            request = new AdRequest.Builder().AddExtra("npa", "1").Build();
        }
        else request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public static void RequestAtStart()
    {
        RequestRewardBasedVideo();
        RequestInterstitialVideo();
    }

    static public bool ShowAdMobRewardedVideo()
    {
        // Load the rewarded video ad with the request.
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
            return true;
        }
        else
        {
            RequestRewardBasedVideo();
            return false;
        }
    }

    public static bool ShowAdMobAD()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            return true;
        }
        else
        {
            RequestInterstitialVideo();
            return false;
        }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        Debug.LogWarning("Rewarded Watched");
        isRewardedWatched = true;
        coinsReward = Convert.ToInt32(args.Amount) > 0 ? Convert.ToInt32(args.Amount) : coinsReward;
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        Debug.LogWarning("Rewarded Closed");
        if (isRewardedWatched)
        {
            isRewardedWatched = false;
            DataManager.Instance.EarnCoins(coinsReward, false);
        }

        AdManager inst = FindObjectOfType<AdManager>();
        inst.SetLoading(false);
        inst.StartTimer();
        RequestRewardBasedVideo();
    }

    private void HandleOnAdClosed(object sender, EventArgs e)
    {
        Debug.LogWarning("EVENT INTERSTITIAL CLOSED");
        RequestInterstitialVideo();
    }

    private void HandleAdLoad(object sender, EventArgs e)
    {
        if (sender.GetType() == typeof(RewardBasedVideoAd))
        {
            Debug.Log("EVENT REWARDED READY");
        }
        else Debug.Log("EVENT INTERSTITIAL READY");
    }
}
#endif



