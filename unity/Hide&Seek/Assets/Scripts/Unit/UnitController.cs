using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public ControlState controlState{ get{ return _state; } }

    public void Initialize(InGameView view, UnitModel unitModel, HideManager hideManager)
    {
        _view = view;
        _unitModel = unitModel;
        _state = ControlState.Move;
        _hideManager = hideManager;

        _view.Initialize(
            onClickActionButton: () => ChangeState(ControlState.Avoid),
            onClickHideButton: () => ChangeState(ControlState.Hide)
        );
    }

    public void Excecute()
    {
        DebugInput();

        switch(_state)
        {
            case ControlState.Move:
                Move();
                break;
            case ControlState.Avoid:
                Avoid();
                break;
            case ControlState.Hide:
                HideStart();
                break;
            case ControlState.HideEnd:
                HideEnd();
                break;
        }

        _oldState = _state;
    }

    private void ChangeState(ControlState state)
    {
        switch(state)
        {
            case ControlState.Move:
                break;
            case ControlState.Avoid:
                if (_state != ControlState.Move)
                    return;
                break;
            case ControlState.Hide:
                if (_state == ControlState.Avoid)
                    return;

                if (_enableHideEnd)
                    state = ControlState.HideEnd;
                break;
        }

        _state = state;
    }

    private void DebugInput()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ChangeState(ControlState.Avoid);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeState(ControlState.Hide);
        }
    }

    private void Move()
    {
        if (_view.GetJoyStick().JoyStickDirection() == Vector2.zero)
        {
            return;
        }

        Vector2 vector = _view.GetJoyStick().JoyStickDirection();

        float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

        Vector3 oldPos = _unitModel.GetTransfrom().localPosition;

        _unitModel.GetTransfrom().localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        _unitModel.GetTransfrom().localPosition += _unitModel.GetTransfrom().localRotation * new Vector3(0.0f, 0.0f, _speed);

        // HIDEボタンの切り替え
        _view.SetInteractableHideButton(_hideManager.enableHideEntity != null);
    }

    private void Avoid()
    {
        if (_oldState != _state)
        {
            _avoidTimer = 0.0f;
        }

        _unitModel.GetTransfrom().localPosition += _unitModel.GetTransfrom().localRotation * new Vector3(0.0f, 0.0f, 0.5f);

        _avoidTimer += Time.deltaTime;

        if (_avoidTimer > _avoidTime)
        {
            _state = ControlState.Move;
        }
    }

    private void HideStart()
    {
        if (_hideManager.enableHideEntity == null)
        {
            _state = ControlState.Move;
            return;
        }

        if (_oldState != _state)
        {
            _hideTimer = 0.0f;
        }

        _unitModel.ModelBanish(true);

        _hideTimer += Time.deltaTime;
        if (_hideTimer > _hideStartWaitTime)
        {
            _enableHideEnd = true;
        }
    }

    private void HideEnd()
    {
        if (_oldState != _state)
        {
            _hideTimer = 0.0f;
        }

        _unitModel.ModelBanish(false);

        _hideTimer += Time.deltaTime;
        if (_hideTimer > _hideEndWaitTime)
        {
            _state = ControlState.Move;
            _enableHideEnd = false;
        }
    }

    public enum ControlState
    {
        Move,
        Avoid,
        Hide,
        HideEnd
    }

    private ControlState _state;
    private ControlState _oldState;

    private InGameView _view;
    private float _speed = 0.05f;
    private UnitModel _unitModel;

    private float _avoidTime = 0.1f;
    private float _avoidTimer = 0.0f;

    private bool _isHide = false;
    private bool _enableHideEnd = false;
    private HideManager _hideManager;

    private float _hideTimer = 0.0f;
    private float _hideStartWaitTime = 0.1f;
    private float _hideEndWaitTime = 0.1f;
}
