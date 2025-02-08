using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollision : MonoBehaviour
{
    public void Initialize(Action action, Transform unitModelTransform)
    {
        _deadCallback = action;
        _unitModelTransform = unitModelTransform;
        _isGameEnd = false;
    }

    public void Excecute()
    {
    
    }

    public void Finish()
    {
    
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isGameEnd)
            return;

        switch(other.tag)
        {
            case "Item":
                var item = other.gameObject.GetComponent<ItemEntity>();
                item.GetItem();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isGameEnd)
            return;

        switch (other.tag)
        {
            case "Enemy":
                if(EnemyDistanceCheck(other.gameObject.transform))
                {
                    _deadCallback();
                }
                break;
        }
    }

    private bool EnemyDistanceCheck(Transform enemyTransform)
    {
        float distance;

        distance = (_unitModelTransform.localPosition - enemyTransform.localPosition).sqrMagnitude;

        return distance < CONTACT_ENEMY * CONTACT_ENEMY;
    }

    private const float CONTACT_ENEMY = 1.0f;

    private Action _deadCallback;
    private Transform _unitModelTransform;
    private bool _isGameEnd;
}
