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
        actionButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
            onClickActionButton?.Invoke();
            SetInteractableButton(actionButton, false);
            Observable.Timer(TimeSpan.FromSeconds(ActionInterval)).Subscribe(_ =>
            {
                SetInteractableButton(actionButton, true);
            }).AddTo(this);
        }).AddTo(this);
        hideButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
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
    private Button actionButton;

    [SerializeField]
    private Button hideButton;

    [SerializeField]
    private Joystick joystick;

    private static readonly float ActionInterval= 3f;
}