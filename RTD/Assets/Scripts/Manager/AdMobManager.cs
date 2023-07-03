using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdMobManager : Singleton<AdMobManager>
{
    public bool isTestMode;

    [HideInInspector]public int reward;

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
        // ÀÌÀü ±¤°í »èÁ¦
        DestroyBannerAd();
        DestroyFrontAd();
        DestroyRewardAd();

        // ±¤°í ·Îµå
        LoadBannerAd();
        LoadFrontAd();
        LoadRewardAd();
    }

    #region ¹è³Ê ±¤°í
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


    #region Àü¸é ±¤°í
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = "ca-app-pub-4059713768893618/6725590211";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdLoaded += (e, sender) =>
        {
            //º¸»ó
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


    #region ¸®¿öµå ±¤°í
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-4059713768893618/6725590211";
    RewardedAd rewardAd;


    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);

        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
            GameManager.Instance.GainGatcha(reward);
        };
        rewardAd.LoadAd(GetAdRequest());
    }

    public void ShowRewardAd(int _reward)
    {
        reward = _reward;
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