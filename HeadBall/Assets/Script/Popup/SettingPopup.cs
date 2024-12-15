using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BasePopup
{
    // sprite short cut
    [SerializeField] Sprite musicOn, musicOff;
    [SerializeField] Sprite soundOn, soundOff;

    [SerializeField] Button closeBtn;
    [SerializeField] Button resumeBtn;
    [SerializeField] Button musicBtn;
    [SerializeField] Button soundBtn;

    private int upright,upleft, downright, downleft;
    private void Awake()
    {
        closeBtn.onClick.AddListener(OnCloseBtn);
        resumeBtn.onClick.AddListener(OnResumeBtn);
        musicBtn.onClick.AddListener(OnMusicBtn);
        soundBtn.onClick.AddListener(OnSoundBtn);
    }

    private void OnDestroy()
    {
        closeBtn.onClick.RemoveListener(OnCloseBtn);
        resumeBtn.onClick.RemoveListener(OnResumeBtn);
        musicBtn.onClick.RemoveListener(OnMusicBtn);
        soundBtn.onClick.RemoveListener(OnSoundBtn);
    }

    private void OnEnable()
    {
        upright = upleft = downright = downleft = 0;
    }

    private void OnCloseBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.CloseSetting(afterCloseCB);
    }

    private void OnResumeBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.CloseSetting(afterCloseCB);
    }

    private void OnMusicBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        int curVol = PlayerPrefs.GetInt(Constant.PREF_MUSIC, 1);
        int newVol = curVol == 1 ? 0 : 1;
        PlayerPrefs.SetInt(Constant.PREF_MUSIC, newVol);
        musicBtn.GetComponent<Image>().sprite = newVol == 1 ? musicOn : musicOff;
        AudioManager.Instance.SetMusicVol(newVol);
    }

    private void OnSoundBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        int curVol = PlayerPrefs.GetInt(Constant.PREF_SOUND, 1);
        int newVol = curVol == 1 ? 0 : 1;
        PlayerPrefs.SetInt(Constant.PREF_SOUND, newVol);
        soundBtn.GetComponent<Image>().sprite = newVol == 1 ? soundOn : soundOff;
        AudioManager.Instance.SetSfxVol(newVol);
    }

    public void OnClickSecret(int i)
    {
#if USE_CHEAT
        switch (i)
        {
            case 0:
                {
                    if (upright == 3)
                    {
                        upleft++;
                    }
                }break;
            case 1:
                {
                    if (upleft == 0 && downleft == 0 && downright == 0)
                    {
                        upright++;
                    }
                }
                break;
            case 2:
                {
                    if (downright == 3)
                    {
                        downleft++;
                        if (downleft == 3)
                        {
                            AudioManager.Instance.PlayButtonClick();
                            MessageBus.Annouce(new Message(MessageBusType.ShowCheatScore));
                        }
                    }
                }
                break;
            case 3:
                {
                    if (upleft == 3)
                    {
                        downright++;
                    }
                }
                break;
        }
        //Debug.LogError($"upright: {upright}, upleft: {upleft}, downright: {downright},downleft: {downleft}");
#endif
    }
}
