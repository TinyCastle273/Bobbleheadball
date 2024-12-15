using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class EffectButtonCtrl : MonoBehaviour, IPointerDownHandler , IPointerClickHandler , IPointerExitHandler, IPointerUpHandler
{
    [System.Serializable] 
    public class ScaleOption
    {
        public Vector3 fixedValue;
        public bool useFixedValue;
    }

    [System.Serializable]
    public class AdditionalImage
    {
        public Image img;
        public Sprite on;
        public Sprite off;
    }


    [SerializeField]
    Transform objBtnScale;
    [SerializeField]
    Graphic graphicColor;
    [SerializeField]
    float duration = 0.2f;

    public ScaleOption scaleOption;


    //public 
    // Use this for initialization
    Vector3 currentScale;
    [SerializeField] bool shouldFade = false;
    [SerializeField] bool useConstantAlpha = false;
    [SerializeField] float currentA = 1f;
    [SerializeField] AdditionalImage[] additionalImages;

    public Vector3 scaleAdd = new Vector3(-0.1f, -0.1f, 0);

    public Action OnDown;
    public Action OnUp;

    private void Awake()
    {
        Init();
    }

    public void OnMyPress()
    {
        Vector3 nextScale = currentScale + scaleAdd;
        objBtnScale.DOKill();
        objBtnScale.DOScale(nextScale, duration).PlayForward();
        foreach(var image in additionalImages)
        {
            image.img.sprite = image.off;
        }
        OnMyEnter();

    }

    public void OnMyReturn()
    {
        objBtnScale.DOKill();
        objBtnScale.DOScale(currentScale, duration).PlayForward();
        foreach (var image in additionalImages)
        {
            image.img.sprite = image.on;
        }
        OnMyExit();
    }

    public void OnMyEnter()
    {
        if (graphicColor == null) return;

        graphicColor.DOKill();
        if (shouldFade)
            graphicColor.DOFade(currentA / 2, duration).PlayForward();
    }

    public void OnMyExit()
    {
        if (graphicColor == null) return;

        graphicColor.DOKill();
        if (shouldFade)
            graphicColor.DOFade(currentA, duration).PlayForward();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnMyPress();
        OnDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMyReturn();
        OnDown?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMyReturn();
        OnUp?.Invoke();
    }

    private void Reset() 
    {
#if UNITY_EDITOR
        Init();
#endif
    }

    private void Init()
    {
        if (objBtnScale == null)
        {
            objBtnScale = transform;
        }

        if (scaleOption != null && scaleOption.useFixedValue)
        {
            currentScale = scaleOption.fixedValue;
        } else {
            currentScale = objBtnScale.localScale;
        }

        if (graphicColor == null) graphicColor = transform.GetComponent<Graphic>();
        if (!useConstantAlpha && graphicColor != null)
        {
            currentA = graphicColor.color.a;
        }
    }

    public void UpdateCurrentAlpha(float alpha)
    {
        currentA = alpha;
    }
}

