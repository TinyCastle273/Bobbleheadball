using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitPopup : BasePopup
{
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
    [SerializeField] Button cancelBtn;

    private void Awake()
    {
        yesBtn.onClick.AddListener(OnYes);
        noBtn.onClick.AddListener(OnNo);
        cancelBtn.onClick.AddListener(OnNo);
    }

    private void OnDestroy()
    {
        yesBtn.onClick.RemoveListener(OnYes);
        noBtn.onClick.RemoveListener(OnNo);
        cancelBtn.onClick.RemoveListener(OnNo);
    }

    private void OnYes()
    {
        AudioManager.Instance.PlayButtonClick();
#if USE_AD
        if (ScenesController.Instance.IsGmapleyScene())
        {
            PopupManager.Instance.CloseQuit(afterCloseCB);
            ScenesController.Instance.LoadMainMenu();
        }
        else
            Application.Quit();
#elif UNITY_WEBGL
        if (ScenesController.Instance.IsGmapleyScene())
        {
            PopupManager.Instance.CloseQuit(afterCloseCB);
            ScenesController.Instance.LoadMainMenu();
        }
        else
            WebGLHelper.QuitWebGL();
#endif
    }

    private void OnNo()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.CloseQuit(afterCloseCB);
    }
}
