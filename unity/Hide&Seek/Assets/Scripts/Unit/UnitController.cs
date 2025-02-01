using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public void Initialize(InGameView view, Transform modelTransform, Camera gameCamera)
    {
        _view = view;
        _modelTransform = modelTransform;
        _gameCamera = gameCamera;
    }

    public void Excecute()
    {
        Debug.Log(_view.GetJoyStick().JoyStickDirection());

        if(_view.GetJoyStick().JoyStickDirection() == Vector2.zero)
        {
            return;
        }

        Vector2 vector = _view.GetJoyStick().JoyStickDirection();

        float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

        Vector3 oldPos = _modelTransform.localPosition;

        _modelTransform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        _modelTransform.localPosition += _modelTransform.localRotation * new Vector3(0.0f, 0.0f, _speed);

        Vector3 movePos = _modelTransform.localPosition - oldPos;
        Vector3 cameraPos = _gameCamera.transform.localPosition;

        cameraPos.x += movePos.x;
        cameraPos.z += movePos.z;

        _gameCamera.transform.localPosition = cameraPos;
    }

    private InGameView _view;
    private float _speed = 0.05f;
    private Transform _modelTransform;
    private Camera _gameCamera;
}
