using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform modelTransform{ get{ return _modelTransform; } }

    public UnitController unitController { get{ return _controller; } }

    public void Initialze(InGameView view, Camera gameCamera, HideManager hideManager)
    {
        _model = gameObject.AddComponent<UnitModel>();
        _model.Initialize();
        _modelTransform = _model.GetTransfrom();

        _controller = gameObject.AddComponent<UnitController>();
        _controller.Initialize(view, _model, hideManager);

        _camera = gameObject.AddComponent<UnitCamera>();
        _camera.Initialize(gameCamera, _modelTransform.localPosition);

        _unitCollision = _modelTransform.gameObject.AddComponent<UnitCollision>();
        _unitCollision.Initialize(Dead, _modelTransform);
    }

    public void Excecute()
    {
        _controller.Excecute();
        _camera.Excecute(_modelTransform.localPosition);
    }

    private void Dead()
    {
        Debug.Log("ASDFADSFASFDASFDA");
    }

    private UnitController _controller;
    private UnitModel _model;
    private UnitCamera _camera;
    private UnitCollision _unitCollision;

    private Transform _modelTransform;
}
