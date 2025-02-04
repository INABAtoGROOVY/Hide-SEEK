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

    //TODO あとで消す
    void Update()
    {
        Execute();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
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
        Debug.LogError(_actionType);
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
    }

    #region 行動
    private void BeginPatrol()
    {
        agent.SetDestination(wayPoints[_targetIndex % wayPoints.Length].position);
        //Debug.LogError($"set destination {targetIndex % wayPoints.Length}");
        _actionType = State.Patrol;
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= 0)
        {
            _targetIndex++;
            _actionType = State.BeginPatrol;
        }
    }

    private void BeginWatch()
    {
        agent.SetDestination(_watchTargetTransform.position);
        _actionType = State.Watch;
    }

    private void Watch()
    {
        if (agent.remainingDistance <= 0)
        {
            _targetIndex++;
            _actionType = State.BeginPatrol;
        }
    }

    private void Chase()
    {
        if (IsHitRaycast(_chaseTargetTransform))
        {
            _overlookedTime += Time.deltaTime;
            if (_overlookedTime >= OverlookedTimeDuration)
            {
                _overlookedTime = 0;
                _actionType = State.BeginPatrol;
            }
        }
        else
        {
            _overlookedTime = 0;
            agent.SetDestination(_chaseTargetTransform.position);
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

    [SerializeField]
    private Transform[] wayPoints;

    [SerializeField]
    private NavMeshAgent agent;

    private int _targetIndex;
    private State _actionType = State.BeginPatrol;
    private Transform _watchTargetTransform;
    private Transform _chaseTargetTransform;
    private float _overlookedTime;

    private static readonly float OverlookedTimeDuration = 3f;
}