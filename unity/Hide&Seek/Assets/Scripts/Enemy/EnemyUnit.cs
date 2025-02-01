using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    public enum ActionType
    {
        BeginPatrol,
        Patrol,
        Watch,
        Chase,
    }

    //TODO つながったら消す
    void Update()
    {
        Execute();
    }

    public void Execute()
    {
        switch (actionType)
        {
            case ActionType.BeginPatrol:
                BeginPatrol();
                break;

            case ActionType.Patrol:
                Patrol();
                break;

            case ActionType.Watch:
                Watch();
                break;

            case ActionType.Chase:
                Chase();
                break;

            default:
                Debug.LogError("undefined action");
                break;
        }
    }

    private void BeginPatrol()
    {
        agent.SetDestination(wayPoints[targetIndex % wayPoints.Length].position);
        //Debug.LogError($"set destination {targetIndex % wayPoints.Length}");
        actionType = ActionType.Patrol;
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= 0)
        {
            targetIndex++;
            actionType = ActionType.BeginPatrol;
        }
    }

    private void Watch()
    {

    }

    private void Chase()
    {

    }

    [SerializeField]
    private Transform[] wayPoints;

    [SerializeField]
    private NavMeshAgent agent;

    private int targetIndex;
    private ActionType actionType = ActionType.BeginPatrol;
}