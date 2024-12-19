using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Cyborg : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void InitSDK(string objectName);
    [DllImport("__Internal")]
    public static extern void StartGame();
    [DllImport("__Internal")]
    public static extern void ShowInterstitialAd();
    [DllImport("__Internal")]
    public static extern void ShowRewardedAd();
    [DllImport("__Internal")]
    public static extern void StopGame();
    [DllImport("__Internal")]
    public static extern void SendTracking(string eventID, string eventParams);
    [DllImport("__Internal")]
    public static extern void GetUserProfile();
    [DllImport("__Internal")]
    public static extern void VerifyJWT();
    [DllImport("__Internal")]
    public static extern void GetBalance(string tokenAddress, string chainID);
    [DllImport("__Internal")]
    public static extern void SendTransaction(string rawTransaction, string toAddress);

    [Serializable]
    public class SDKInitCallbackEvent : UnityEvent<string> { }
    public class GetUserProfileCallbackEvent : UnityEvent<string> { }
    public class VerifyJWTCallbackEvent : UnityEvent<string> { }
    public SDKInitCallbackEvent onSDKInitCallback = new SDKInitCallbackEvent();
    public GetUserProfileCallbackEvent onUserProfileCallback = new GetUserProfileCallbackEvent();
    public VerifyJWTCallbackEvent onVerifyJWTCallback = new VerifyJWTCallbackEvent();

    public void SDKInitCallback(string result)
    {
        onSDKInitCallback.Invoke(result);
    }

    public void GetUserProfileCallback(string userProfileString)
    {
        onUserProfileCallback.Invoke(userProfileString);
    }

    public virtual void ShowRewardedAdCallback(string userProfileString)
    {
        // TODO 
    }

    public void VerifyJWTCallback(string jwtProfileString)
    {
        onVerifyJWTCallback.Invoke(jwtProfileString);
    }
}
