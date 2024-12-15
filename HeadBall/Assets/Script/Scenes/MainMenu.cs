using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [Header("Charater")]
    [SerializeField] RectTransform character;
    [SerializeField] TextMeshProUGUI characterNameTxt;
    [SerializeField] CanvasGroup nameCanvas;
    [SerializeField] CanvasGroup dialog;
    [SerializeField] Animator animator;
    [SerializeField] Image headImg;
    [SerializeField] Image handImg;
    [SerializeField] Image feetImg;

    [Header("Group Button")]
    [SerializeField] Button startMatchBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button playFirstBtn;
    [SerializeField] Button creditBtn;
    [SerializeField] Button coinBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button careerBtn;
    [SerializeField] TextMeshProUGUI coinTxt;

    [Header("Group Background")]
    [SerializeField] CanvasGroup bg1Group;
    [SerializeField] GameObject bg2Group;
    [SerializeField] float animTime = 0.35f;
    [SerializeField] float timeShowDialog = 3f;

    private void Awake()
    {
        startMatchBtn.onClick.AddListener(OnStartMatch);
        backBtn.onClick.AddListener(OnBackBtn);
        playFirstBtn.onClick.AddListener(OnPlayFirstBtn);
        creditBtn.onClick.AddListener(OnCreditBtn);
        coinBtn.onClick.AddListener(OnCoinBtn);
        settingBtn.onClick.AddListener(OnSettingBtn);
        careerBtn.onClick.AddListener(OnCareerBtn);

#if UNITY_WEBGL
        careerBtn.gameObject.SetActive(false);
        startMatchBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, startMatchBtn.GetComponent<RectTransform>().anchoredPosition.y);
#endif
    }

    private void OnDestroy()
    {
        startMatchBtn.onClick.RemoveListener(OnStartMatch);
        backBtn.onClick.RemoveListener(OnBackBtn);
        playFirstBtn.onClick.RemoveListener(OnPlayFirstBtn);
        creditBtn.onClick?.RemoveListener(OnCreditBtn);
        coinBtn.onClick.RemoveListener(OnCoinBtn);
        settingBtn.onClick.RemoveListener(OnSettingBtn);
        careerBtn.onClick.RemoveListener(OnCareerBtn);
    }

    private void OnEnable()
    {
        var userData = GameManager.Instance.GetUserData();
        headImg.sprite = userData.head;
        handImg.sprite = userData.hand;
        feetImg.sprite = userData.feet;
        characterNameTxt.text = userData.characterName;
        coinTxt.text = GameManager.Instance.GetTotalCoin().ToString();

        if (ScenesController.Instance.IsFromLoading)
        {
            ScenesController.Instance.IsFromLoading = false;
            bg1Group.gameObject.SetActive(true);
            character.anchoredPosition = new Vector2(0f, 85f);
            character.transform.localScale = Vector3.one;
            playFirstBtn.interactable = true;
            nameCanvas.alpha = 0f;
        }
        else
        {
            bg1Group.gameObject.SetActive(false);
            character.anchoredPosition = new Vector2(-534f, 85f);
            character.transform.localScale = new Vector3(0.72f, 0.72f);
            nameCanvas.alpha = 1f;
            StartCoroutine(IERandomShowDialog());
        }
        dialog.alpha = 0f;
    }

    private void OnBackBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.OpenQuit();
    }

    private void OnStartMatch()
    {
        AudioManager.Instance.PlayButtonClick();
#if UNITY_WEBGL
        ApiManager.Instance.Play((res) =>
        {
            if (GameManager.Instance.GetEnemyData() != null)
            {
                GameManager.Instance.CacheTimePlayed();
                ScenesController.Instance.LoadGameplay();
            }
        },
        (error) =>
        {
            if (error.errorCode.Equals("ASSET_EXPIRED"))
            {
                GameManager.Instance.CacheTimePlayed();
                ScenesController.Instance.LoadGameplay();
            }
            else
                PopupManager.Instance.OpenError(error.message);
        });
#else
        if (GameManager.Instance.GetEnemyData() != null)
        {
            GameManager.Instance.CacheTimePlayed();
            ScenesController.Instance.LoadGameplay();
        }
#endif
    }

    private void OnPlayFirstBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        animator.enabled = false;
        bg1Group.DOFade(0f, animTime).OnComplete(() => { 
            bg1Group.gameObject.SetActive(false); 
            animator.enabled = true;
            nameCanvas.DOFade(1f, animTime);
            StartCoroutine(IERandomShowDialog());
        });
        character.DOAnchorPos(new Vector2(-534f, 85f), animTime);
        character.DOScale(0.72f, animTime);
        playFirstBtn.interactable = false;  // avoid user click again
    }

    private void OnCreditBtn()
    {
        AudioManager.Instance.PlayButtonClick();
#if USE_AD
        AdManager.Instance.ShowRewardAd();
#endif
        PopupManager.Instance.OpenCredit();
    }

    private void OnCoinBtn()
    {
        AudioManager.Instance.PlayButtonClick();
    }

    private void OnSettingBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.OpenSetting();
    }

    private void OnCareerBtn()
    {
        AudioManager.Instance.PlayButtonClick();
    }

    IEnumerator IERandomShowDialog()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeShowDialog*1.5f, timeShowDialog*3f));
            var showDialogSeq = DOTween.Sequence();
            showDialogSeq.Append(dialog.DOFade(1f, animTime));
            showDialogSeq.AppendInterval(timeShowDialog);
            showDialogSeq.Append(dialog.DOFade(0f, animTime));
        }
    }
}
