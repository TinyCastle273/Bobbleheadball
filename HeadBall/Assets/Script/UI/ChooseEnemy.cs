using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChooseEnemy : MonoBehaviour
{
    [SerializeField] Image avatarImg;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] Button avatarBtn;
    [SerializeField] CharacterSO character;
    [SerializeField] float animTime = 0.15f;

    private Image btnImg;
    private int state;
    private void Awake()
    {
        avatarBtn.onClick.AddListener(OnClick);
        btnImg = avatarBtn.GetComponent<Image>();
        MessageBus.Instance.Subscribe(MessageBusType.SelectEnemy, OnSelectEnemy);
        MessageBus.Instance.Subscribe(MessageBusType.DeselectEnemy, OnDeselectEnemy);
    }

    private void OnDestroy()
    {
        MessageBus.Instance.Unsubscribe(MessageBusType.SelectEnemy, OnSelectEnemy);
        MessageBus.Instance.Unsubscribe(MessageBusType.DeselectEnemy, OnDeselectEnemy);
    }

    private void Start()
    {
        InitData();
    }

    private void OnClick()
    {
        if (state == 1)
        {
            MessageBus.Annouce(new Message(MessageBusType.DeselectEnemy, character));
            GameManager.Instance.SetEnemyData(null);
        }  
        else
        {
            MessageBus.Annouce(new Message(MessageBusType.SelectEnemy, character));
            GameManager.Instance.SetEnemyData(character);
        }
    }

    private void OnSelectEnemy(Message msg)
    {
        if (msg != null)
        {
            var data = (CharacterSO)msg.data;
            if (data == character)
            {
                ChangeState(1);
            }
            else
            {
                ChangeState(2);
            }
        }
    }
    private void OnDeselectEnemy(Message msg)
    {
        if (msg != null)
        {
            ChangeState(0);
        }
    }


    private void ChangeState(int state)
    {
        this.state = state;
        switch (state)
        {
            case 0: // normal
                {
                    btnImg.DOColor(Color.white, animTime);
                    transform.DOScale(1f, animTime);
                } break;
            case 1: // select
                {
                    btnImg.DOColor(Color.green, animTime);
                    transform.DOScale(1.1f, animTime);
                }
                break;
            case 2: // deselect
                {
                    btnImg.DOColor(Color.white, animTime);
                    transform.DOScale(0.85f, animTime);
                }
                break;
        }
    }

    public void SetCharacterData(CharacterSO data)
    {
        character = data;
        InitData();
    }

    private void InitData()
    {
        if (character != null)
        {
            avatarImg.sprite = character.avatar;
            nameTxt.text = character.characterName;
        }
        else
        {
            avatarImg.sprite = null;
            nameTxt.text = string.Empty;
        }
    }
}
