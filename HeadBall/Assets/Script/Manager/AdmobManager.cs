using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobManager : SingletonMono<AdmobManager>
{
    [SerializeField]private BannerAdController _bannerAdController;
    [SerializeField]private IntersititialAdController _intersititialAdController;

    public void ShowInterstitialAd()
    {
        _intersititialAdController.ShowInterstitialAd();
    }

    public void ShowBannerAd()
    {
        _bannerAdController.LoadAd();
    }

    public void HideBannerAd()
    {
        _bannerAdController.DestroyAd();
    }
}
