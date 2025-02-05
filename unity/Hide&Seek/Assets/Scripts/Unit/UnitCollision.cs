using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollision : MonoBehaviour
{
    public void Initialize(Action action)
    {
        _deadCallback = action;
    }

    void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Enemy":
                _deadCallback();
                break;
            case "Item":
                var item = other.gameObject.GetComponent<ItemEntity>();
                item.GetItem();
                break;
        }
    }

    Action _deadCallback;
}
