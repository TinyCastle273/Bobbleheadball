using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    [SerializeField] bool isLeft;
    [SerializeField] AudioClip goalWin;
    [SerializeField] AudioClip goalLose;

    private bool isRoundEnd;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe(MessageBusType.RoundEnd, OnRoundEnd);
        MessageBus.Instance.Subscribe(MessageBusType.NewRound, OnNewRound);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe(MessageBusType.RoundEnd, OnRoundEnd);
        MessageBus.Instance.Unsubscribe(MessageBusType.NewRound, OnNewRound);
    }

    private void OnRoundEnd(Message msg)
    {
        isRoundEnd = true;
    }

    private void OnNewRound(Message msg)
    {
        isRoundEnd = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRoundEnd) return;

        if (collision.CompareTag(Constant.TAG_BALL))
        {
            MessageBus.Annouce(new Message(MessageBusType.ScoreGoal, isLeft));
            if (isLeft) AudioManager.Instance.PlaySfx(goalLose, 0.8f);
            else AudioManager.Instance.PlaySfx(goalWin, 0.8f);
        }
    }
}
