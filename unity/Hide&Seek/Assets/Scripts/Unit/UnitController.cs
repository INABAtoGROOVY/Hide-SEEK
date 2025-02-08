using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public ControlState controlState{ get{ return _state; } }

    public void Initialize(InGameView view, Transform modelTransform)
    {
        _view = view;
        _modelTransform = modelTransform;
        _state = ControlState.Move;

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
                //Hide();
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
            case ControlState.Hide:
                if (_state != ControlState.Move)
                    return;
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

        Vector3 oldPos = _modelTransform.localPosition;

        _modelTransform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        _modelTransform.localPosition += _modelTransform.localRotation * new Vector3(0.0f, 0.0f, _speed);
    }

    private void Avoid()
    {
        if (_oldState != _state)
        {
            _avoidTimer = 0.0f;
        }

        _modelTransform.localPosition += _modelTransform.localRotation * new Vector3(0.0f, 0.0f, 0.5f);

        _avoidTimer += Time.deltaTime;

        if (_avoidTimer > _avoidTime)
        {
            _state = ControlState.Move;
        }
    }

    private void HideStart()
    {
        _state = ControlState.Move;
    }

    private void HideEnd()
    {
    
    }

    public enum ControlState
    {
        Move,
        Avoid,
        Hide
    }

    private ControlState _state;
    private ControlState _oldState;

    private InGameView _view;
    private float _speed = 0.05f;
    private Transform _modelTransform;

    private float _avoidTime = 0.1f;
    private float _avoidTimer = 0.0f;

    private bool _isHide = false;
    private HideManager _hideManager;
}
