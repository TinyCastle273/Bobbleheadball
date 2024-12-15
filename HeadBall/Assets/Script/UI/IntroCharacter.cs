using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCharacter : MonoBehaviour
{
    [SerializeField] CharacterSO character;
    [SerializeField] Image headImg;
    [SerializeField] Image handImg;
    [SerializeField] Image feetImg;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        ApplyData();
    }

    public void SetData(CharacterSO data)
    {
        character = data;
        ApplyData();
    }

    private void ApplyData()
    {
        if (character != null)
        {
            headImg.sprite = character.head;
            handImg.sprite = character.hand;
            feetImg.sprite = character.feet;
        }
    }

    public RectTransform GetRect() => rect;
}
