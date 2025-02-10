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
        SetInteractableButton(_actionButton, true);
        _actionButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
            if (!_actionButton.interactable) return;

            onClickActionButton?.Invoke();
            SetInteractableButton(_actionButton, false);
            Observable.Timer(TimeSpan.FromSeconds(ActionInterval)).Subscribe(_ =>
            {
                SetInteractableButton(_actionButton, true);
            }).AddTo(this);
        }).AddTo(this);

        SetInteractableButton(_hideButton, false);
        _hideButton.OnPointerClickAsObservable().Subscribe(_ =>
        {
            if (!_hideButton.interactable) return;
            _hideButton.isHold = !_hideButton.isHold;
            _hideButton.SetText(_hideButton.isHold ? HideEndText : HideText);
            onClickHideButton?.Invoke();
        }).AddTo(this);
    }

    public void SetTimerUI(int time)
    {
        _timer.ApplyTime(time);
    }

    public void SetItemView(int current, int limit)
    {
        _itemCountFrameView.Apply(current, limit);
    }

    public void SetActiveInGameUI(bool isActive)
    {
        _actionButton.gameObject.SetActive(isActive);
        _hideButton.gameObject.SetActive(isActive);
        _joystick.gameObject.SetActive(isActive);
        _heaerGroup.gameObject.SetActive(isActive);
    }

    public void SetInteractableHideButton(bool isInteractable) => SetInteractableButton(_hideButton, isInteractable);

    public IJoyStick GetJoyStick() => _joystick;

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
    private HoldableButton _actionButton;

    [SerializeField]
    private HoldableButton _hideButton;

    [SerializeField]
    private GameObject _heaerGroup;

    [SerializeField]
    private Timer _timer;

    [SerializeField]
    private ItemCountFrameView _itemCountFrameView;

    [SerializeField]
    private Joystick _joystick;

    [SerializeField]
    private GameObject _startReadyObj;

    [SerializeField]
    private TextMeshProUGUI _startReadyText;

    private static readonly float ActionInterval = 3f;
    private static readonly string HideText = "HIDE";
    private static readonly string HideEndText = "HIDE\nEND";
}