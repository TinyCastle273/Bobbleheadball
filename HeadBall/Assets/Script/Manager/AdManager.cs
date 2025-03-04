using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : SingletonMono<AdManager>
{
    private Action _onRewardSuccess;
    private Action _onRewardFailed;
    private void Start()
    {
        GameDistribution.Instance.PreloadRewardedAd();
    }

    void Awake()
    {
        GameDistribution.OnResumeGame += OnResumeGame;
        GameDistribution.OnPauseGame += OnPauseGame;
        GameDistribution.OnPreloadRewardedVideo += OnPreloadRewardedVideo;
        GameDistribution.OnRewardedVideoSuccess += OnRewardedVideoSuccess;
        GameDistribution.OnRewardedVideoFailure += OnRewardedVideoFailure;
        GameDistribution.OnRewardGame += OnRewardGame;
    }

    public void OnResumeGame()
    {
        // RESUME MY GAME
        AudioManager.Instance.EnableSound(true);
        ScenesController.Instance.PauseGame(false);
    }

    public void OnPauseGame()
    {
        // PAUSE MY GAME
        AudioManager.Instance.EnableSound(false);
        ScenesController.Instance.PauseGame(true);
    }

    public void OnRewardGame()
    {
        // REWARD PLAYER HERE
        AudioManager.Instance.EnableSound(true);
        this._onRewardSuccess?.Invoke();
        this._onRewardSuccess = null;

    }

    public void OnRewardedVideoSuccess()
    {
        // Rewarded video succeeded/completed.;
        AudioManager.Instance.EnableSound(true);

    }

    public void OnRewardedVideoFailure()
    {
        // Rewarded video failed.;
        AudioManager.Instance.EnableSound(true);
        this._onRewardFailed?.Invoke();
        this._onRewardFailed = null;
    }

    public void OnPreloadRewardedVideo(int loaded)
    {
        // Feedback about preloading ad after called GameDistribution.Instance.PreloadRewardedAd
        // 0: SDK couldn't preload ad
        // 1: SDK preloaded ad
    }

    public void ShowAd()
    {
        AudioManager.Instance.EnableSound(false);
        GameDistribution.Instance.ShowAd();
#if UNITY_EDITOR
        Debug.Log("Show Ad");
#endif
    }

    public void ShowRewardedAd(Action success, Action failed = null)
    {
        AudioManager.Instance.EnableSound(false);
        this._onRewardFailed = failed;
        this._onRewardSuccess = success;
        this.PreloadRewardedAd();
        GameDistribution.Instance.ShowRewardedAd();
    }

    public void PreloadRewardedAd()
    {
        GameDistribution.Instance.PreloadRewardedAd();
    }
}
