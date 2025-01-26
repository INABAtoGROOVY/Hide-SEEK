using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    private int targetIndex;

    [SerializeField]
    private Transform[] wayPoints;

    [SerializeField]
    private NavMeshAgent agent;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            agent.SetDestination(wayPoints[targetIndex % wayPoints.Length].position);
            //Debug.LogError($"set destination {targetIndex % wayPoints.Length}");

            yield return null;
            while (agent.remainingDistance > 0)
            {
                yield return null;
            }
            targetIndex++;

        }
    }
}