using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
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

    public void SetTimerUI(int time)
    {
        timer.ApplyTime(time);
    }

    public void SetItemView(int current, int limit)
    {
        itemCountFrameView.Apply(current, limit);
    }

    public void SetActiveInGameUI(bool isActive)
    {
        actionButton.gameObject.SetActive(isActive);
        hideButton.gameObject.SetActive(isActive);
        joystick.gameObject.SetActive(isActive);
        _heaerGroup.gameObject.SetActive(isActive);
    }

    public void SetInteractableHideButton(bool isInteractable) => SetInteractableButton(hideButton, isInteractable);

    public IJoyStick GetJoyStick() => joystick;

    public void SetActiveStartReadyUI(bool isActive)
    {
        _startReadyObj.SetActive(isActive);
    }

    public void SetStartReadyText(string text)
    {
        _startReadyText.text = text;
    }

    private void SetInteractableButton(Button button, bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    [SerializeField]
    private HoldableButton actionButton;

    [SerializeField]
    private HoldableButton hideButton;

    [SerializeField]
    private GameObject _heaerGroup;

    [SerializeField]
    private Timer timer;

    [SerializeField]
    private ItemCountFrameView itemCountFrameView;

    [SerializeField]
    private Joystick joystick;

    [SerializeField]
    private GameObject _startReadyObj;

    [SerializeField]
    private TextMeshProUGUI _startReadyText;

    private static readonly float ActionInterval = 3f;
    private static readonly string HideText = "HIDE";
    private static readonly string HideEndText = "HIDE\nEND";
}