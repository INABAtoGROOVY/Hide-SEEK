using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModel : MonoBehaviour
{
    public void Initialize()
    {
        _meshObj = transform.GetChild(0).gameObject;
        _modelObj = transform.GetChild(1).gameObject;
        _animator = GetComponent<Animator>();
    }

    public Transform GetTransfrom()
    {
        return _modelObj.transform;
    }

    public void SetAnimatorMove(bool isMove)
    {
        _animator.SetBool("isMove", isMove);
    }

    public void ModelBanish(bool isBanish)
    {
        _meshObj.SetActive(!isBanish);
    }

    private GameObject _meshObj;
    private GameObject _modelObj;
    private Animator _animator;
}
