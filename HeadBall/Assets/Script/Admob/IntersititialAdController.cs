using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class IntersititialAdController : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    //private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
    private string _adUnitId = "ca-app-pub-1443690986008985/5837487942";

#elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-1443690986008985/8826479916";
#else
      private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            DestroyAds();
        }

        Debug.Log("Load Interstitial Ads");

        var adRequest = new AdRequest();

        InterstitialAd.Load(this._adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                 "with error : " + error);
                    return;
                }

                this._interstitialAd = ad;
                this.RegisterReloadHandler(_interstitialAd);
            }
        );
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }

    private void DestroyAds()
    {
        _interstitialAd.Destroy();
        _interstitialAd = null;
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
}
