using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    public void Initialize(
        Action onClickActionButton,
        Action onClickHideButton
    )
    {
        timer.ApplyTime(100);

        SetInteractableButton(actionButton, true);
        actionButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
            if (!actionButton.interactable) return;

            onClickActionButton?.Invoke();
            SetInteractableButton(actionButton, false);
            Observable.Timer(TimeSpan.FromSeconds(ActionInterval)).Subscribe(_ =>
            {
                SetInteractableButton(actionButton, true);
            }).AddTo(this);
        }).AddTo(this);

        SetInteractableButton(hideButton, false);
        hideButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
            if (!hideButton.interactable) return;
            hideButton.isHold = !hideButton.isHold;
            hideButton.SetText(hideButton.isHold ? HideEndText : HideText);
            onClickHideButton?.Invoke();
        }).AddTo(this);
    }

    public void SetInteractableHideButton(bool isInteractable) => SetInteractableButton(hideButton, isInteractable);

    public IJoyStick GetJoyStick() => joystick;

    private void SetInteractableButton(Button button, bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    [SerializeField]
    private HoldableButton actionButton;

    [SerializeField]
    private HoldableButton hideButton;

    [SerializeField]
    private Timer timer;

    [SerializeField]
    private Joystick joystick;

    private static readonly float ActionInterval = 3f;
    private static readonly string HideText = "HIDE";
    private static readonly string HideEndText = "HIDE\nEND";
}