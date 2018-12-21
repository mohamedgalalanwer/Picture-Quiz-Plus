using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AdsIapSettings : ScriptableObject //Root ScriptableObject that representes Ads settings instance in the Inspector. Check InspectorGUI class that is in symbiosis with it
{
    public bool unityAds = false;
    public bool unityIap = false;
    public bool adMob = false;
    public bool GDPRconsent = false;
    public bool nonpersonalizedAds = false;

    //Set fields with default values at start
    public bool isUnityIapSettingOpened = false;
    public bool isUnityAdsSettingOpened = false;
    public bool isUnityAdMobSettingOpened = false;

    public string ProductIDConsumable = "add_coins";
    public int coinsAdReward = 100;
    public int coinsIAPReward = 1000;
    public int showAdAfterLevel = 0;
    public string delayBetweenAds = "00:00:10";
    public string adMobAndroidAppID = "";
    public string adMobAndroidRewardedID = "ca-app-pub-3940256099942544/5224354917";
    public string adMobAndroidInterstitialID = "ca-app-pub-3940256099942544/1033173712";
    public string adMobIosAppID = "";
    public string adMobIosRewardedID = "ca-app-pub-3940256099942544/1712485313";
    public string adMobIosInterstitialID = "ca-app-pub-3940256099942544/4411468910";


}
