using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : SingletonMono<AdManager>
{
    private Action _onRewardSuccess;
    private Action _onRewardFailed;
    private void Awake()
    {
        GameMonetize.OnPauseGame += OnPauseGame;
        GameMonetize.OnResumeGame += OnResumeGame;
    }

    private void OnDisable()
    {
        GameMonetize.OnPauseGame -= OnPauseGame;
        GameMonetize.OnResumeGame -= OnResumeGame;
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

    public void ShowAd()
    {
        AudioManager.Instance.EnableSound(false);
        GameMonetize.Instance.ShowAd();
#if UNITY_EDITOR
        Debug.Log("Show Ad");
#endif
    }


}
