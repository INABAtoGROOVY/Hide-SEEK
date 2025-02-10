using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public ControlState controlState{ get{ return _state; } }

    public void Initialize(InGameView view, UnitModel unitModel, HideManager hideManager)
    {
        _view = view;
        _unitModel = unitModel;
        _unitTransform = unitModel.GetTransfrom();

        _state = ControlState.Move;
        _hideManager = hideManager;

        _view.Initialize(
            onClickActionButton: () => ChangeState(ControlState.Avoid),
            onClickHideButton: () => ChangeState(ControlState.Hide)
        );

        _agent = _unitTransform.GetComponent<NavMeshAgent>();

        _moveRecordLoop = null;
        _moveReplayLoop = null;

        SetupAgentMove(RecorderSystem.Instance.isReplay);
    }

    public void Excecute()
    {
        if(!RecorderSystem.Instance.isReplay && _moveRecordLoop == null)
        {
            _moveRecordLoop = MoveRecord();
            StartCoroutine(_moveRecordLoop);
        }

        if(RecorderSystem.Instance.isReplay && _moveReplayLoop == null)
        {
            _moveReplayLoop = MoveReplay();
            StartCoroutine(_moveReplayLoop);
        }

        if (RecorderSystem.Instance.isReplay)
        {
            ReplayInput();

            if (_agent.hasPath)
            {
                _agent.velocity = _agent.desiredVelocity.normalized * _agent.speed;
            }
        }

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

    public void SetGameTime(float gameTime)
    {
        _gameTime = gameTime;
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

                RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitAvoid, _unitTransform.localPosition.x, _unitTransform.localPosition.z, _unitTransform.localEulerAngles.y);
                break;
            case ControlState.Hide:
                if (_state == ControlState.Avoid)
                    return;

                if (_enableHideEnd)
                {
                    state = ControlState.HideEnd;
                    RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitHideEnd, _unitTransform.localPosition.x, _unitTransform.localPosition.z, _unitTransform.localEulerAngles.y);
                }
                else
                {
                    RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitHide, _unitTransform.localPosition.x, _unitTransform.localPosition.z, _unitTransform.localEulerAngles.y);
                }
                break;
        }

        _state = state;
    }

    private void ReplayInput()
    {
            var list = RecorderSystem.Instance.NextActionRecord(_gameTime);

            if (list == null)
                return;

            for (int idx = 0; idx < list.Count; idx++)
            {
                switch (list[idx].recordType)
                {
                    case RecordData.RecordType.UnitAvoid:
                        _agent.Warp(new Vector3(list[idx].param1, _agent.transform.localPosition.y, list[idx].param2));
                        _agent.transform.localRotation = Quaternion.Euler(new Vector3(0, list[idx].param3, 0));
                        ChangeState(ControlState.Avoid);
                        break;
                    case RecordData.RecordType.UnitHide:
                        _agent.Warp(new Vector3(list[idx].param1, _agent.transform.localPosition.y, list[idx].param2));
                        _agent.transform.localRotation = Quaternion.Euler(new Vector3(0, list[idx].param3, 0));
                        ChangeState(ControlState.Hide);
                        break;
                    case RecordData.RecordType.UnitHideEnd:
                        _agent.Warp(new Vector3(list[idx].param1, _agent.transform.localPosition.y, list[idx].param2));
                        _agent.transform.localRotation = Quaternion.Euler(new Vector3(0, list[idx].param3, 0));
                        ChangeState(ControlState.HideEnd);
                        break;
                }
            }
        
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
        Vector2 vector;

        if(RecorderSystem.Instance.isReplay)
        {
            vector = _replayJoystickVec;

            if (vector == Vector2.zero)
            {
                return;
            }
        }
        else
        {
            if (_view.GetJoyStick().JoyStickDirection() == Vector2.zero)
            {
                return;
            }

            vector = _view.GetJoyStick().JoyStickDirection();
        }

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

        if(RecorderSystem.Instance.isReplay)
        {
            SetupAgentMove(false);
        }

        _unitModel.GetTransfrom().localPosition += _unitModel.GetTransfrom().localRotation * new Vector3(0.0f, 0.0f, 0.5f);

        _avoidTimer += Time.deltaTime;

        if (_avoidTimer > _avoidTime)
        {
            if (RecorderSystem.Instance.isReplay)
            {
                SetupAgentMove(true);
            }

            _state = ControlState.Move;
        }
    }

    private void HideStart()
    {
        if (_hideManager.enableHideEntity == null && !RecorderSystem.Instance.isReplay)
        {
            _state = ControlState.Move;
            return;
        }

        if (_oldState != _state)
        {
            _hideTimer = 0.0f;
        }

        if (RecorderSystem.Instance.isReplay)
        {
            SetupAgentMove(false);
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

        RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitHideEnd, _unitTransform.localPosition.x, _unitTransform.localPosition.z, _unitTransform.localEulerAngles.y);

        _unitModel.ModelBanish(false);

        _hideTimer += Time.deltaTime;
        if (_hideTimer > _hideEndWaitTime)
        {
            if (RecorderSystem.Instance.isReplay)
            {
                SetupAgentMove(true);
            }

            _state = ControlState.Move;
            _enableHideEnd = false;
        }
    }

    private IEnumerator MoveRecord()
    {
        while (true)
        {
            yield return new WaitForSeconds(RecorderSystem.Instance.collection.moveRecordInterval);
            _replayJoystickVec = _view.GetJoyStick().JoyStickDirection();
            RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitMove, _replayJoystickVec.x, _replayJoystickVec.y, _unitTransform.localEulerAngles.y);

            //RecorderSystem.Instance.AddRecord(_gameTime, RecordData.RecordType.UnitMove, _unitTransform.localPosition.x, _unitTransform.localPosition.z, _unitTransform.localEulerAngles.y);
        }
    }

    private IEnumerator MoveReplay()
    {
        while (true)
        {
            yield return new WaitForSeconds(RecorderSystem.Instance.collection.moveRecordInterval);

            var record = RecorderSystem.Instance.NextMoveRecord();

            if (record == null)
                continue;

            _replayJoystickVec = new Vector2(record.param1, record.param2);
            /*
            _unitTransform.localPosition = new Vector3(record.param1, _unitTransform.localPosition.y, record.param2);
            _agent.transform.localRotation = Quaternion.Euler(new Vector3(0, record.param3, 0));
            */
            //_agent.SetDestination(new Vector3(record.param1, _agent.pathEndPosition.y, record.param2));
        }
    }

    private void SetupAgentMove(bool isActive)
    {
        if(isActive)
        {
            _agent.speed = 6f;
            _agent.acceleration = 0f;
            _agent.angularSpeed = 999f;
            _agent.stoppingDistance = 0;
            _agent.isStopped = false;
        }
        else
        {
            _agent.speed = 0f;
            _agent.isStopped = true;
        }
    }

    public bool IsHide()
    {
        return controlState == ControlState.Hide;
    }

    public bool IsInvinsible()
    {
        return controlState == ControlState.Avoid || controlState == ControlState.Hide;
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
    private float _speed = 0.1f;
    private UnitModel _unitModel;
    private Transform _unitTransform;

    private float _avoidTime = 0.1f;
    private float _avoidTimer = 0.0f;

    private bool _enableHideEnd = false;
    private HideManager _hideManager;

    private float _hideTimer = 0.0f;
    private float _hideStartWaitTime = 0.1f;
    private float _hideEndWaitTime = 0.1f;

    private float _gameTime;
    private NavMeshAgent _agent;
    private IEnumerator _moveRecordLoop;
    private IEnumerator _moveReplayLoop;
    private Vector2 _replayJoystickVec = Vector2.zero;
}
