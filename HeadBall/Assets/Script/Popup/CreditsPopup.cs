using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsPopup : BasePopup
{
    [SerializeField] CanvasGroup[] canvasGroups = default;
    [SerializeField] Button bgBtn;
    [SerializeField] float delayStart = 3f;
    [SerializeField] float animDuration = 2f;
    [SerializeField] float transition = 2.5f;

    private bool isAnim;
    private void OnEnable()
    {
        bgBtn.onClick.AddListener(OnBgClick);
        isAnim = true;
        foreach(CanvasGroup group in canvasGroups)
        {
            group.alpha = 0f;
        }
        DoAnimation();
    }

    private void DoAnimation()
    {
        StartCoroutine(IEAnimation());
    }

    private IEnumerator IEAnimation()
    {
        yield return new WaitForSeconds(delayStart);
        foreach (CanvasGroup group in canvasGroups)
        {
            group.DOFade(1f, animDuration);
            yield return new WaitForSeconds(animDuration + transition);
        }
        isAnim = false;
    }

    private void OnBgClick()
    {
        if (isAnim)
        {
            StopAllCoroutines();
            foreach (CanvasGroup group in canvasGroups)
            {
                group.DOFade(1f, 0.5f).OnComplete(() => isAnim = false);
            }
        }
        else
        {
            PopupManager.Instance.CloseCredit(afterCloseCB);
        }
    }
}
