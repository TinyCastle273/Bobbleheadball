using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameplayScene : SingletonMono<GameplayScene>
{
    [Header("Editor Only")]
    [SerializeField] private int ScoreToWin = 10;
    [SerializeField] private bool showIntro;
    [SerializeField] private bool skipCountDown;

    [Space]
    [SerializeField] RectTransform canvas;
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;
    [Space]
   

    [Header("Intro Character")]
    [SerializeField] IntroCharacter player;
    [SerializeField] IntroCharacter enemy;
    [SerializeField] RectTransform foreImg;
    [SerializeField] float animMoveIntroChar = 0.35f;
    [SerializeField] float animDelayMoveIn = 0.5f;

    [Header("User Info")]
    [SerializeField] Image faceLeftImg;
    [SerializeField] Image faceRightImg;
    [SerializeField] TextMeshProUGUI leftNameTxt;
    [SerializeField] TextMeshProUGUI rightNameTxt;
    [SerializeField] TextMeshProUGUI leftScoreTxt;
    [SerializeField] TextMeshProUGUI rightScoreTxt;

    [Header("Countdown")]
    [SerializeField] Image number3;
    [SerializeField] Image number2;
    [SerializeField] Image number1;
    [SerializeField] TextMeshProUGUI goTxt;

    [Header("Audio Clip")]
    [SerializeField] AudioClip countSfx;
    [SerializeField] AudioClip whistleBegin;
    [SerializeField] AudioClip whistleFinal;
    [SerializeField] AudioClip[] crowdSfx;
    [SerializeField] AudioClip[] clashSfx;

    [Header("Gameplay")]
    [SerializeField] Vector3[] playerPosition;
    [SerializeField] BallController ballController;
    [SerializeField] PlayerCharacter[] players;
    [SerializeField] GameObject blackPanel;
    [SerializeField] RectTransform finalWhistle;

    [Header("Cheat")]
    [SerializeField] Button scoreLeftBtn;
    [SerializeField] Button scoreRightBtn;
    [SerializeField] Button endBtn;

    private CharacterSO playerData;
    private CharacterSO enemyData;
    private int scoreLeft, scoreRight;

    private void Awake()
    {
        settingBtn.onClick.AddListener(OnSettingClicked);
        quitBtn.onClick.AddListener(OnQuitClicked);

        scoreLeftBtn.gameObject.SetActive(false);
        scoreRightBtn.gameObject.SetActive(false);
        endBtn.gameObject.SetActive(false);

        MessageBus.Instance.Subscribe(MessageBusType.ShowCheatScore, OnShowCheatScore);
    }

    protected override void OnDestroy()
    {
        settingBtn.onClick.RemoveListener(OnSettingClicked);
        quitBtn.onClick.RemoveListener(OnQuitClicked);
        MessageBus.Instance.Subscribe(MessageBusType.ShowCheatScore, OnShowCheatScore);
        base.OnDestroy();
    }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe(MessageBusType.ScoreGoal, OnScoreGoal);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe(MessageBusType.ScoreGoal, OnScoreGoal);
        base.OnDestroy();
    }

    private void Start()
    {
        playerData = GameManager.Instance.GetUserData();
        enemyData = GameManager.Instance.GetEnemyData();

        foreach (var player in players)
            player.SetEnable(false);
        ballController.gameObject.SetActive(false);
        ResetPlayerPos();

        if (showIntro)
        {
            foreImg.gameObject.SetActive(true);
            player.SetData(playerData);
            enemy.SetData(enemyData);

            player.GetRect().DOAnchorPos(new Vector2(-596f, player.GetRect().anchoredPosition.y), animMoveIntroChar).SetEase(Ease.OutBack).SetDelay(animDelayMoveIn);
            enemy.GetRect().DOAnchorPos(new Vector2(596f, enemy.GetRect().anchoredPosition.y), animMoveIntroChar).SetEase(Ease.OutBack).SetDelay(animDelayMoveIn).OnComplete(() =>
            {
                AudioManager.Instance.PlayRandomSfx(clashSfx, 0.25f);
                foreImg.DOAnchorPos(new Vector2(0f, -canvas.sizeDelta.y), animMoveIntroChar * 0.75f).SetEase(Ease.InQuad).SetDelay(2f).OnComplete(() =>
                {
                    foreImg.gameObject.SetActive(false);
                    StartCountDown();
                });
            });
        }
        else
        {
            foreImg.gameObject.SetActive(false);
            StartCountDown();
        }

        scoreLeft = scoreRight = 0;
        faceLeftImg.sprite = playerData.head;
        leftNameTxt.text = playerData.characterName;
        faceRightImg.sprite = enemyData.head;
        rightNameTxt.text = enemyData.characterName;

        StartCoroutine(MakeTheCrowds());
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        leftScoreTxt.text = scoreLeft.ToString();
        rightScoreTxt.text = scoreRight.ToString();
    }

    private void StartCountDown()
    {
        if (!skipCountDown)
        {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() => AudioManager.Instance.PlaySfx(countSfx));
            seq.Append(number3.transform.DOScale(2f, 0.125f)).SetEase(Ease.InQuad);
            seq.AppendInterval(0.5f);
            seq.Append(number3.transform.DOScale(0f, 0.15f));
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() => AudioManager.Instance.PlaySfx(countSfx));
            seq.Append(number2.transform.DOScale(2f, 0.65f)).SetEase(Ease.InQuad);
            seq.AppendInterval(0.5f);
            seq.Append(number2.transform.DOScale(0f, 0.15f));
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() => AudioManager.Instance.PlaySfx(countSfx));
            seq.Append(number1.transform.DOScale(2f, 0.65f)).SetEase(Ease.InQuad);
            seq.AppendInterval(0.5f);
            seq.Append(number1.transform.DOScale(0f, 0.15f));
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() => AudioManager.Instance.PlaySfx(whistleBegin, 0.75f));
            seq.Append(goTxt.transform.DOScale(1f, 0.65f)).SetEase(Ease.InQuad);
            seq.AppendInterval(0.5f);
            seq.Append(goTxt.transform.DOScale(0f, 0.15f));
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                number3.gameObject.SetActive(false);
                number2.gameObject.SetActive(false);
                number1.gameObject.SetActive(false);
                goTxt.gameObject.SetActive(false);
                foreach (var player in players)
                    player.SetEnable(true);
                ballController.PrepareTheBall(Vector3.zero);
            });
        }
        else
        {
            number3.gameObject.SetActive(false);
            number2.gameObject.SetActive(false);
            number1.gameObject.SetActive(false);
            goTxt.gameObject.SetActive(false);
            foreach (var player in players)
                player.SetEnable(true);
            ballController.PrepareTheBall(Vector3.zero);
        }
    }

    private void OnScoreGoal(Message msg)
    {
        // which one is scored
        var isLeft = (bool)msg.data;

        if (isLeft) ++scoreRight;
        else ++scoreLeft;
        UpdateScoreText();
        CheckGame(isLeft);
    }

    private void CheckGame(bool isLeft)
    {
        foreach (var player in players)
            player.SetEnable(false);
        MessageBus.Annouce(new Message(MessageBusType.RoundEnd));
        if (scoreRight >= ScoreToWin || scoreLeft >= ScoreToWin)
        {
            // finish game
            blackPanel.SetActive(true);

            StopAllCoroutines();
            AudioManager.Instance.StopMusic();
            
            finalWhistle.transform.localScale = Vector3.zero;
            finalWhistle.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(1f).OnComplete(() => AudioManager.Instance.PlaySfx(whistleFinal, 0.75f));

            GameManager.Instance.CheckWinCondition(scoreLeft, scoreRight);
            StartCoroutine(IEWaitForResultScreen());
        }
        else
        {
#if USE_AD
            if (ScoreToWin - scoreRight == 1 || ScoreToWin - scoreLeft == 1)
            {
                AdManager.Instance.LoadInterAd();
            }
#endif
            StartCoroutine(IEForSecond(isLeft));
        }
    }

    private void ResetPlayerPos()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = playerPosition[i];
        }
    }

    IEnumerator IEForSecond(bool isLeft)
    {
        yield return new WaitForSeconds(3.5f);
        ballController.PrepareTheBall(isLeft ? playerPosition[0] : playerPosition[1]);
        ResetPlayerPos();
        foreach (var player in players)
            player.SetEnable(true);
        MessageBus.Annouce(new Message(MessageBusType.NewRound));
    }

    IEnumerator  IEWaitForResultScreen()
    {
        yield return new WaitForSeconds(5f);
#if USE_AD
            AdManager.Instance.ShowInterAd();
#endif
        if (GameManager.Instance.GetCoinWin() == 0)
        {
            ScenesController.Instance.LoadMainMenu();
        }
        else
        {
            ScenesController.Instance.LoadReward();
        }
    }

    IEnumerator MakeTheCrowds()
    {
        while (true)
        {
            AudioManager.Instance.PlayRandomMusic(crowdSfx, 0.25f);
            // time of crown audio
            yield return new WaitForSeconds(Random.Range(20f, 26f));
        }
    }

    private void OnSettingClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.OpenSetting(() => ScenesController.Instance.PauseGame(true));

    }

    private void OnQuitClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        PopupManager.Instance.OpenQuit(() => ScenesController.Instance.PauseGame(true));
    }

    public void IncreaseScore(bool isLeft)
    {
        if (isLeft) ++scoreLeft;
        else ++scoreRight;
        UpdateScoreText();
    }

    public void OnEndGameClick()
    {
        CheckGame(true);
    }

    private void OnShowCheatScore(Message msg)
    {
        scoreLeftBtn.gameObject.SetActive(true);
        scoreRightBtn.gameObject.SetActive(true);
        endBtn.gameObject.SetActive(true);
    }
}
