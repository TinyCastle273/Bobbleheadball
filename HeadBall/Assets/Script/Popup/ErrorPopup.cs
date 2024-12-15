using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPopup : BasePopup
{
    [SerializeField] TextMeshProUGUI errorTxt;

    public string error;

    private void OnEnable()
    {
        errorTxt.text = error;
    }

    public void OnClose()
    {
        PopupManager.Instance.CloseError();
    }
}
