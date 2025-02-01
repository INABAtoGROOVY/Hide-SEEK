using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public void Initialze(InGameView view, Camera gameCamera)
    {
        _model = gameObject.AddComponent<UnitModel>();
        _model.Initialize();

        _controller = gameObject.AddComponent<UnitController>();
        _controller.Initialize(view, _model.GetTransfrom(), gameCamera);
        
    }

    public void Excecute()
    {
        _controller.Excecute();
    }

    private UnitController _controller;
    private UnitModel _model;
}
