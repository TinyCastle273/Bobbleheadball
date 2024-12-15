using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPause : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLHelper.registerVisibilityChangeEvent();
#endif
    }

    void OnVisibilityChange(string visibilityState)
    {
        if (visibilityState == "visible")
        {
            ScenesController.Instance.PauseGame(false);
            AudioManager.Instance.EnableSound(true);
        }
        else
        {
            AudioManager.Instance.EnableSound(false);
            ScenesController.Instance.PauseGame(true);
        }
    }

#if UNITY_ANDROID
    private void OnApplicationPause(bool pause)
    {
        ScenesController.Instance.PauseGame(pause);
    }
#endif
}
