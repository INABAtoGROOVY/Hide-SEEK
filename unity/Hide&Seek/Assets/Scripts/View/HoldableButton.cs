using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class HoldableButton : Button
{
    new void Start()
    {
        _isHold = new(default);
        _isHold.Subscribe(isHold =>
        {
            targetGraphic.color = isHold ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
            GetComponentInChildren<TextMeshProUGUI>().color = isHold ? Color.white : Color.black;
        }).AddTo(this);
    }

    public void SetText(string text)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public bool isHold
    {
        get => _isHold.Value;
        set => _isHold.Value = value;
    }
    private ReactiveProperty<bool> _isHold;
}
