using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] RectTransform left;
    [SerializeField] RectTransform right;
    [SerializeField] RectTransform logo;
    [SerializeField] Slider progressBar;
    [SerializeField] float maxFakeTime = 4f;
    [SerializeField] AudioClip[] clashClip;

    private float fakeTime = 0;
    private float counter = 0;
    private int finishLoading;
    private bool getAccessToken = false;
    private void Awake()
    {
        fakeTime = UnityEngine.Random.Range(2f, maxFakeTime);
        progressBar.value = 0f;

        logo.localScale = Vector3.zero;
        left.anchoredPosition = new Vector2(-1400f, 34f);
        right.anchoredPosition = new Vector2(1400f, 30f);

        getAccessToken = true;
    }

    private void Start()
    {
        if (!getAccessToken) return;

        FakeLoading();
        finishLoading = 2;
    }

    private void FakeLoading()
    {
        StartCoroutine(IELoading());
        DoAnimation();
    }

    IEnumerator IELoading()
    {
        finishLoading = 0;
        yield return new WaitForSeconds(0.25f);
        while (counter <= fakeTime)
        {
            var nxtVal = UnityEngine.Random.Range(progressBar.value, 0.8f);
            progressBar.DOValue(nxtVal, 0.25f);
            counter += 1f;
            yield return new WaitForSeconds(1f);
        }
#if UNITY_ANDROID
        yield return new WaitForSeconds(0.25f);
        finishLoading = 2;
#endif
        yield return new WaitUntil(() => finishLoading >= 2);
        progressBar.value = 1f;
        AfterLoading();
    }

    private void AfterLoading()
    {
        ScenesController.Instance.LoadMainMenu();
    }

    private void DoAnimation()
    {
        left.DOAnchorPos(new Vector2(-598f, 34f), 1f).SetEase(Ease.OutBack);
        right.DOAnchorPos(new Vector2(598f, 30f), 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            AudioManager.Instance.PlayRandomSfx(clashClip);
            logo.DOScale(1f, 1f).SetEase(Ease.OutBack);
        });
    }
}
