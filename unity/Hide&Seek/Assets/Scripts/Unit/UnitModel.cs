using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModel : MonoBehaviour
{
    public void Initialize()
    {
        _modelObj = transform.GetChild(1).gameObject;
    }

    public Transform GetTransfrom()
    {
        return _modelObj.transform;
    }

    private GameObject _modelObj;
    private Animator _animator;
}
