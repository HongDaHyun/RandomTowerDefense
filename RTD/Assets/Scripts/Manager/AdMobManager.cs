using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdMobManager : Singleton<AdMobManager>
{
    public bool isTestMode;

    void Start()
    {
        var requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(new List<string>() { "DA55FBA1AEDF47FA" }).build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadBannerAd();
        LoadFrontAd();
        LoadRewardAd();
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이전 광고 삭제
        DestroyBannerAd();
        DestroyFrontAd();
        DestroyRewardAd();

        // 광고 로드
        LoadBannerAd();
        LoadFrontAd();
        LoadRewardAd();
    }

    #region 배너 광고
    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "ca-app-pub-4059713768893618/2456825105";
    BannerView bannerAd;


    void LoadBannerAd()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            return;
        AdSize adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID,
            adaptiveSize, AdPosition.Top);
        bannerAd.LoadAd(GetAdRequest());
    }

    public void ToggleBannerAd(bool b)
    {
        if (b) bannerAd.Show();
        else bannerAd.Hide();
    }

    void DestroyBannerAd()
    {
        if (bannerAd != null)
        {
            bannerAd.Destroy();
            bannerAd = null;
        }
    }
    #endregion


    #region 전면 광고
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = "ca-app-pub-4059713768893618/6725590211";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdLoaded += (e, sender) =>
        {
            //보상
        };
    }

    public void ShowFrontAd()
    {
        frontAd.Show();
        LoadFrontAd();
    }

    void DestroyFrontAd()
    {
        if (frontAd != null)
        {
            frontAd.Destroy();
            frontAd = null;
        }
    }
    #endregion


    #region 리워드 광고
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-4059713768893618/6725590211";
    RewardedAd rewardAd;


    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);

        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
            //보상
        };
    }

    public void ShowRewardAd()
    {
        rewardAd.Show();
        LoadRewardAd();
    }

    void DestroyRewardAd()
    {
        if (rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }
    }
    #endregion
}