using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PopupManager : SingletonMono<PopupManager>
{
    [SerializeField] CreditsPopup creditsPopup;
    [SerializeField] SettingPopup settingPopup;
    [SerializeField] QuitPopup quitPopup;
    [SerializeField] ErrorPopup errorPopup;

    [SerializeField] RectTransform fader;
    [SerializeField] float animTime = 0.5f;

    private bool isAnimating = false;
    private void OpenPopup(Transform trans, Action act)
    {
        if (isAnimating) return;
        isAnimating = true;
        trans.gameObject.SetActive(true);
        fader.gameObject.SetActive(true);
        trans.localScale = Vector3.zero;
        trans.DOScale(1f, animTime).SetEase(Ease.OutBack).OnComplete(() => OnCompleteAnim(act));
    }

    private void ClosePopup(Transform trans, Action act)
    {
        if (isAnimating) return;
        isAnimating = true;
        trans.DOScale(0f, animTime).SetEase(Ease.InBack).OnComplete(() => 
        {
            OnCompleteAnim(act);
            fader.gameObject.SetActive(false);
            trans.gameObject.SetActive(false);
        });
    }

    public void OpenSetting(Action action = null)
    {
        OpenPopup(settingPopup.transform, action);
    }

    public void CloseSetting(Action action = null)
    {
        if (ScenesController.Instance.IsPause)
            ScenesController.Instance.PauseGame(false);
        ClosePopup(settingPopup.transform, action);
    }

    public void OpenCredit(Action action = null)
    {
        OpenPopup(creditsPopup.transform, action);
    }

    public void CloseCredit(Action action = null)
    {
        ClosePopup(creditsPopup.transform, action);
    } 

    public void OpenQuit(Action action = null)
    {
        OpenPopup(quitPopup.transform, action);
    }

    public void CloseQuit(Action action = null)
    {
        if (ScenesController.Instance.IsPause)
            ScenesController.Instance.PauseGame(false);
        ClosePopup(quitPopup.transform, action);
    }

    public void OpenError(string error, Action action = null)
    {
        errorPopup.error = error;
        OpenPopup(errorPopup.transform, action);
    }

    public void CloseError(Action action = null)
    {
        errorPopup.error = string.Empty;
        ClosePopup(errorPopup.transform, action);
    }

    private void OnCompleteAnim(Action act)
    {
        isAnimating = false;
        act?.Invoke();
    }
}
 