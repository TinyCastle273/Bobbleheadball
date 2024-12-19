using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using UnityEngine;

public class CyborgSDK : Cyborg
{
    [SerializeField] private string APIKey;
    [SerializeField] private string gameSlug;
    [SerializeField] private ServerType serverType;
    [SerializeField] private string stagingUrl = "https://cyborg-api-stg.coin98.tech";
    [SerializeField] private string productionUrl = "https://api.cyborg.game";

    private UserProfile _userProfile;
    public static CyborgSDK Instance;

    private Action<bool> showRewardedAdCallback;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);

#if !UNITY_EDITOR && UNITY_WEBGL
        InitSDK(gameObject.name);
#endif

    }

    private void OnEnable()
    {
        onSDKInitCallback.AddListener(internal_onSDKInitCallback);

    }
    private void OnDisable()
    {
        onSDKInitCallback.RemoveListener(internal_onSDKInitCallback);
    }

    public void SDKShowInterstitialAd()
    {

#if !UNITY_EDITOR && UNITY_WEBGL
        ShowInterstitialAd();
#endif
    }

    public void SDKStartGame()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        StartGame();
#endif
    }

    public void SDKEndGame()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        StopGame();
#endif
    }

    private void internal_onSDKInitCallback(string result)
    {
        try
        {
            _userProfile = JsonUtility.FromJson<UserProfile>(result);
            Debug.Log("Get User Profile Success!");
        }
        catch (Exception e)
        {
            Debug.Log("Get User Profile Failed: " + e.Message);
        }

    }


    public void ShowRewardedAd(Action<bool> callback)
    {
        showRewardedAdCallback = callback;
    }

    public override void ShowRewardedAdCallback(string userProfileString)
    {
        bool showAdsSuccess = (userProfileString.ToUpper().Contains("SUCCESS"));
        showRewardedAdCallback?.Invoke(showAdsSuccess);
        base.ShowRewardedAdCallback(userProfileString);

    }

    public void UpdateMissionInprogress()
    {
        if (_userProfile == null) return;
        UpdateMission(new MissionData()
        {
            code = "1",
            status = "in_progress",
            address = _userProfile.userId,
            completedAt = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
        });
    }

    public void UpdateMissionCompleted()
    {
        if (_userProfile == null) return;
        UpdateMission(new MissionData()
        {
            code = "1",
            status = "completed",
            address = _userProfile.userId,
            completedAt = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
        });
    }


    public async void UpdateMission(MissionData missionData)
    {
        string url = serverType == ServerType.Production ? productionUrl : stagingUrl;
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{url}/v1/partner/game/{gameSlug}/mission");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            APIKey);
        string jsonContent = JsonUtility.ToJson(missionData);
        Debug.Log("jsonContent " + jsonContent);
        var content = new StringContent(jsonContent);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(response.StatusCode);

    }

    [Serializable]
    public struct MissionData
    {
        public string code;
        public string status;
        public string address;
        public string completedAt;
    }

    [Serializable]
    public class UserProfile
    {
        public string userId;
    }

    [Serializable]
    public enum ServerType
    {
        Staging, Production
    }
}

