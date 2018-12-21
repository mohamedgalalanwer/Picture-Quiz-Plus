using System;
using System.Collections;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.UI;

public class AdManager : MonoBehaviour //Class to manage advertisements
{
    public Text timeLabel; //Text component of countdown
    private Button adButton;
    private int coinsReward; //Reward for AD
    private string timeBtwnADs; //Time between ADs

    //Variables that are needed for logic
    private bool isADseen;
    private double tcounter;
    private DateTime currentTime;
    private TimeSpan _remainingTime;
    private DateTime endTime;
    private string Timeformat;
    private bool timerSet;
    private bool countIsReady;
    //private string placementId = "rewardedVideo";
    private bool usingUnityAds;
    private bool usingAdmob;
    private bool usingMixed = false;
    private bool loading;

    void Awake()
    {
        usingUnityAds = DataManager.Instance.adsSettings.unityAds;
        usingAdmob = DataManager.Instance.adsSettings.adMob;
        if (usingUnityAds && usingAdmob)
        {
            usingMixed = true;
        }
        coinsReward = DataManager.Instance.adsSettings.coinsAdReward;
        timeBtwnADs = DataManager.Instance.adsSettings.delayBetweenAds;
        adButton = GetComponent<Button>();
        adButton.onClick.AddListener(ShowAd);//Add OnClick method to the AD button
        endTime = DateTime.Parse(PlayerPrefs.GetString("endTime", currentTime.ToString()));
        CheckAd();
    }

    private void OnEnable()
    {
        countIsReady = false;
        isADseen = false;
        SetLoading(false);
        CheckAd();
    }

    void CheckAd() //Is AD ready to be shown
    {
        UpdateTime();

        if (currentTime < endTime && (endTime.Subtract(currentTime) <= TimeSpan.Parse(timeBtwnADs)))
        {
            isADseen = true;
        }
        else EnableButton("+" + coinsReward.ToString());
    }

    public void StartTimer() //Start countdown after watching AD
    {
        UpdateTime();
        endTime = currentTime + TimeSpan.Parse(timeBtwnADs);
        PlayerPrefs.SetString("endTime", endTime.ToString());
        isADseen = true;
    }

    private void UpdateTime()
    {
        currentTime = DateTime.Now;
        timerSet = true;
    }

    void Update()
    {

        if (isADseen && timerSet) // Check is AD already seen
        {
            if (currentTime < endTime) //Start countdown if there is remaining time
            {
                _remainingTime = endTime.Subtract(currentTime);
                tcounter = _remainingTime.TotalMilliseconds;
                countIsReady = true;
            }
            else
            {
                isADseen = false;
            }
        }
        else if (!loading)
        {
            EnableButton("+" + coinsReward.ToString()); //Activate button after countdown
        }
        if (countIsReady)
        {
            StartCountdown(); //Start countdown if there is remaining time
        }
    }

    public string GetRemainingTime(double x)
    {
        TimeSpan tempB = TimeSpan.FromMilliseconds(x);
        Timeformat = string.Format("{0:D2}:{1:D2}", tempB.Minutes, tempB.Seconds);
        return Timeformat;
    }

    private void StartCountdown() //Logic to display countdown on the AD button
    {
        timerSet = false;
        tcounter -= Time.deltaTime * 1000;
        DisableButton(GetRemainingTime(tcounter));
        if (tcounter <= 0)
        {
            countIsReady = false;
            UpdateTime();
        }
    }

    private void EnableButton(string x)
    {
        adButton.interactable = true;
        timeLabel.text = x;
    }

    private void DisableButton(string x)
    {
        adButton.interactable = false;
        timeLabel.text = x;
    }

    void ShowAd()
    {
        if (usingMixed) //When unity ads and AdMob enabled togather
        {
#if UNITY_ADS && ENABLE_ADMOB
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            int rng = UnityEngine.Random.Range(0, 2);
            if (rng == 1) //Chose AD network randomly with 50/50 chanse
            {
                StartLoading();

                if (!ShowUnityAdsRewardedVideo())
                {
                    AdMob.ShowAdMobRewardedVideo();
                }
            }
            else
            {
                StartLoading();
                if (!AdMob.ShowAdMobRewardedVideo())
                {
                    ShowUnityAdsRewardedVideo();
                }
            }
#endif
        }
        else
        {
            if (usingUnityAds)
            {
#if UNITY_ADS
                StartLoading();
                ShowUnityAdsRewardedVideo();
#endif
            }
            else if (usingAdmob)
            {
#if ENABLE_ADMOB
                StartLoading();
                AdMob.ShowAdMobRewardedVideo();
#endif
            }
        }
    }

    private void StartLoading()
    {
        timerSet = false;
        SetLoading(true);
        DisableButton("...");
    }

    public void SetLoading(bool value)
    {
        transform.Find("noConnection").gameObject.SetActive(value);
        loading = value;
    }


#if UNITY_ADS
    private bool ShowUnityAdsRewardedVideo()
    {
        if (Advertisement.IsReady("rewardedVideo")) //Show rewarded video if ready
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show("rewardedVideo", options);
            return true;
        }
        else
        {
            StartLoading();
            return false;
        }
    }

    void HandleShowResult(ShowResult result) //Handle AD result
    {
        if (result == ShowResult.Finished)
        {
            DataManager.Instance.EarnCoins(coinsReward, true); //Add coins when AD successfully watched
            SetLoading(false);// And start countdown
            StartTimer();
        }
        else if (result == ShowResult.Skipped)
        {
            SetLoading(false);
            //Handle skipped AD if nessesary
        }
        else if (result == ShowResult.Failed)
        {
            SetLoading(false);
            //Debug.LogError("Video failed to show, isAD initialize =" + Advertisement.isInitialized);
        }
    }
#endif

}