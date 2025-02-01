using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCamera : MonoBehaviour
{
    public void Initialize(Camera camera, Vector3 modelPos)
    {
        _gameCamera = camera;
        _oldPos = modelPos;
    }

    public void Excecute(Vector3 modelPos)
    {
        Vector3 movePos = modelPos - _oldPos;
        Vector3 cameraPos = _gameCamera.transform.localPosition;

        cameraPos.x += movePos.x;
        cameraPos.z += movePos.z;

        _gameCamera.transform.localPosition = cameraPos;
        _oldPos = modelPos;
    }

    private Camera _gameCamera;
    private Vector3 _oldPos;
}
