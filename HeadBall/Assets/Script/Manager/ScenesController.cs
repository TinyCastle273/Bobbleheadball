using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : SingletonMono<ScenesController>
{
    [SerializeField] int sceneIdxToStart = 1;
    [SerializeField] AudioClip[] bgMusic;
    // default scene is persistent scene
    private int sceneIdx = 0;

    // stupid but fast
    public bool IsFromLoading;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChange;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        // load splash screen first
        SceneManager.LoadScene(sceneIdxToStart, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneIdx > 0)
            SceneManager.UnloadSceneAsync(sceneIdx);
        sceneIdx = scene.buildIndex;
        if (sceneIdx == 3 || sceneIdx == 5)
        {
#if USE_AD
            //AdManager.Instance.ShowBanner();
            AdmobManager.Instance.ShowBannerAd();
#endif
            AudioManager.Instance.PlayRandomMusic(bgMusic, 0.25f);
        }
        else
        {
#if USE_AD
            //AdManager.Instance.HideBanner();
            AdmobManager.Instance.HideBannerAd();
#endif
        }
        SceneManager.SetActiveScene(scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    private void OnActiveSceneChange(Scene prev, Scene next)
    {

    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
    }

    public void LoadLoadingScene()
    {
        IsFromLoading = true;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    public void LoadReward()
    {
        SceneManager.LoadSceneAsync(5, LoadSceneMode.Additive);
    }

    public void LoadGameplay()
    {
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Additive);
    }

    private bool isPause;
    public bool IsPause
    {
        get { return isPause; }
    }

    public void PauseGame(bool pause)
    {
        isPause = pause;
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public bool IsGmapleyScene() => sceneIdx == 4;
}
