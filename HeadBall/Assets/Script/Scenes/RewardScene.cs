using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RewardScene : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] CanvasGroup menuGroup;
    [SerializeField] Button menuBtn;

    private void Awake()
    {
        panel.localScale = Vector3.zero;
        coinTxt.text = "0";
        GameManager.Instance.AddCoinReward(GameManager.Instance.GetCoinWin());
        menuGroup.alpha = 0;
        menuBtn.onClick.AddListener(OnMenuBtnClick);
    }

    private void OnDestroy()
    {
        menuBtn.onClick.RemoveListener(OnMenuBtnClick);
    }

    private void Start()
    {
        DoAnimation();
    }

    private void OnMenuBtnClick()
    {
        ScenesController.Instance.LoadMainMenu();
    }

    private void DoAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(panel.DOScale(1f, 1.5f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() => coinTxt.text = "+" + GameManager.Instance.GetCoinWin().ToString());
        seq.Append(coinTxt.transform.DOScale(1.1f, 0.25f).SetEase(Ease.OutBack));
        seq.Append(coinTxt.transform.DOScale(1f, 0.1f));
        seq.Append(menuGroup.DOFade(1f, 1.25f));
        seq.Play();
    }
}
