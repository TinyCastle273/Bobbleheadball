using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMono<AudioManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip btnClicked;
    [SerializeField] AudioListener listener;

    public void StopMusic() {
        musicSource.Stop();
    }

    public void PlayMusic(AudioClip clip, float vol = 1f)
    {
        musicSource.volume = vol;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip, float vol = 1f)
    {
        sfxSource.volume = vol;
        sfxSource.PlayOneShot(clip, vol);    
    }

    public void PlayRandomSfx(AudioClip[] clpis, float vol = 1f)
    {
        var rand = Random.Range(0, clpis.Length);
        PlaySfx(clpis[rand], vol);
    }

    public void PlayRandomMusic(AudioClip[] clpis, float vol = 1f)
    {
        var rand = Random.Range(0, clpis.Length);
        PlayMusic(clpis[rand], vol);
    }

    public void SetMusicVol(int val)
    {
        mixer.SetFloat("musicVol", val == 1 ? 0f : -80f);
    }

    public void SetSfxVol(int val)
    {
        mixer.SetFloat("sfxVol", val == 1 ? 0f : -80f);
    }

    public void PlayButtonClick()
    {
        PlaySfx(btnClicked);
    }

    public void EnableSound(bool enable)
    {
#if UNITY_WEBGL
        mixer.SetFloat("musicVol", enable ? 0f : -80f);
        mixer.SetFloat("sfxVol", enable ? 0f : -80f);
#else
        listener.enabled = enable;
#endif
    }
}
