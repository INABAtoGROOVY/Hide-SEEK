using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public void Initialze(InGameView view, Camera gameCamera)
    {
        _model = gameObject.AddComponent<UnitModel>();
        _model.Initialize();
        _modelTransform = _model.GetTransfrom();

        _controller = gameObject.AddComponent<UnitController>();
        _controller.Initialize(view, _modelTransform);

        _camera = gameObject.AddComponent<UnitCamera>();
        _camera.Initialize(gameCamera, _modelTransform.localPosition);
    }

    public void Excecute()
    {
        _controller.Excecute();
        _camera.Excecute(_modelTransform.localPosition);
    }

    private UnitController _controller;
    private UnitModel _model;
    private UnitCamera _camera;

    private Transform _modelTransform;
}
