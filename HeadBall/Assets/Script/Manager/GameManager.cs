using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    [SerializeField] CharacterSO userData;
    [SerializeField] CharacterSO enemyData;

    private int coinToWin = 0;
    private float startPlayedTime = 0f;
    private void Start()
    {
        MessageBus.Instance.Initialize();
    }

    public CharacterSO GetUserData()
    {
        return userData;
    }



    public int GetTotalCoin()
    {
        return 0;
    }

    public void AddCoinReward(int coin)
    {

        PlayerPrefs.SetInt(Constant.PREF_COIN, PlayerPrefs.GetInt(Constant.PREF_COIN, 0));
        PlayerPrefs.Save();
    }

    public void SetEnemyData(CharacterSO data)
    {
        enemyData = data;
    }

    public CharacterSO GetEnemyData()
    {
        return enemyData;
    }

    public bool IsEnemyDataValid()
    {
        return enemyData != null;
    }

    public void CheckWinCondition(int scoreLeft, int scoreRight)
    {
        coinToWin = 0;
        if (scoreLeft > scoreRight)
        {
            var sub = scoreLeft - scoreRight;
            if (sub >= 10) coinToWin = 3;
            else if (sub >= 5) coinToWin = 2;
            else if (sub >= 1) coinToWin = 1;
        }
    }

    public int GetCoinWin() => coinToWin;

    public void CacheTimePlayed() 
    {
        startPlayedTime = Time.realtimeSinceStartup;
    }

    public float GetPlayedTime() => Time.realtimeSinceStartup - startPlayedTime;
}
