using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    public void Initalize(Action callback)
    {
        _finishCallback = callback;
    }

    public void Excecute()
    {
        if (_isGot)
            return;

        TurnItem();
    }

    public void GetItem()
    {
        _isGot = true;
        _finishCallback();

        Destroy(gameObject);
    }

    private void TurnItem()
    {
        float yRotate = transform.localEulerAngles.y;
        yRotate += 1;
        transform.localRotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, yRotate, 0));
    }

    private bool _isGot = false;
    private Action _finishCallback;
}
