using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    public enum State
    {
        BeginPatrol,
        Patrol,
        BeginWatch,
        Watch,
        Chase,
    }

    public void Initialize(GameObject wayPointObj, int initTargetIndex = 0)
    {
        int wayPointCount = wayPointObj.transform.childCount;
        _initTargetIndex = initTargetIndex;
        _targetIndex = initTargetIndex;

        _wayPoints = new Transform[wayPointCount];
        for (int idx = 0; idx < wayPointCount; idx++)
        {
            _wayPoints[idx] = wayPointObj.transform.GetChild(idx).transform;
        }

        _agent.speed = 0;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.transform.GetComponentInParent<UnitController>().IsHide())
            {
                return;
            }

            if (IsHitRaycast(other.transform))
            {
                _watchTargetTransform = other.transform;
                _actionType = State.BeginWatch;
            }
            else
            {
                _chaseTargetTransform = other.transform;
                _actionType = State.Chase;
            }
        }
    }

    public void Execute()
    {
        //Debug.LogError(_actionType);
        switch (_actionType)
        {
            case State.BeginPatrol:
                BeginPatrol();
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.BeginWatch:
                BeginWatch();
                break;

            case State.Watch:
                Watch();
                break;

            case State.Chase:
                Chase();
                break;

            default:
                Debug.LogError("undefined action");
                break;
        }

        SetSpeed(_actionType);
    }

    #region 行動
    private void BeginPatrol()
    {
        _agent.SetDestination(_wayPoints[_targetIndex % _wayPoints.Length].position);
        //Debug.LogError($"set destination {targetIndex % wayPoints.Length}");
        _actionType = State.Patrol;
    }

    private void Patrol()
    {
        if (_agent.remainingDistance <= 0.01f)
        {
            _targetIndex++;
            _actionType = State.BeginPatrol;
        }
    }

    private void BeginWatch()
    {
        _agent.SetDestination(_watchTargetTransform.position);
        _actionType = State.Watch;
    }

    private void Watch()
    {
        if (_agent.remainingDistance <= 0)
        {
            _targetIndex++;
            _actionType = State.BeginPatrol;
        }
    }

    private void Chase()
    {
        if (IsHitRaycast(_chaseTargetTransform) || _chaseTargetTransform.GetComponentInParent<UnitController>().IsHide())
        {
            _overlookedTime += Time.deltaTime;
            if (_overlookedTime >= OverlookedTimeDuration)
            {
                _overlookedTime = 0;
                _targetIndex = _initTargetIndex;
                _actionType = State.BeginPatrol;
            }
        }
        else
        {
            _overlookedTime = 0;
            _agent.SetDestination(_chaseTargetTransform.position);
        }
    }
    #endregion

    private bool IsHitRaycast(Transform target)
    {
        var diff = target.transform.position - transform.position;
        RaycastHit hit;
        var ray = Physics.Raycast(transform.position, diff.normalized, out hit, diff.magnitude);
        return ray && hit.collider.tag != "Player";
    }

    private void SetSpeed(State state)
    {
        _speed = state switch
        {
            State.BeginPatrol => DefaultSpeed,
            State.Patrol => DefaultSpeed,
            State.BeginWatch => DefaultSpeed,
            State.Watch => DefaultSpeed,
            State.Chase => ChaseSpeed,
            _ => DefaultSpeed,
        };

        _agent.speed = _speed;
    }

    [SerializeField]
    private NavMeshAgent _agent;

    private Transform[] _wayPoints;
    private int _targetIndex;
    private int _initTargetIndex;
    private State _actionType = State.BeginPatrol;
    private Transform _watchTargetTransform;
    private Transform _chaseTargetTransform;
    private float _overlookedTime;
    private float _speed;

    private static readonly float OverlookedTimeDuration = 3f;
    private static readonly float DefaultSpeed = 4f;
    private static readonly float ChaseSpeed = 15f;
}