using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePix;
public class AdManager : SingletonMono<AdManager>
{
    public void ShowAd()
    {
        Gpx.Ads.InterstitialAd(OnInterstitalAdSuccess);
    }

    [AOT.MonoPInvokeCallback(typeof(Gpx.gpxCallback))]
    public static void OnInterstitalAdSuccess()
    {
        Gpx.Log("SUCCESS");
    }

}
