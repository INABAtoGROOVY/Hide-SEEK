using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideManager : MonoBehaviour
{
    public HideEntity enableHideEntity { get{ return _enableHideEntity; } }

    public void Initialize(Transform unitModelTransform)
    {
        for (int idx = 0; idx < _hideEntityPosList.Count; idx++)
        {
            HideEntity hideEntity = Instantiate(_hideObj, transform).GetComponent<HideEntity>();
            
            hideEntity.transform.localPosition = _hideEntityPosList[idx];
            hideEntity.transform.localRotation = Quaternion.identity;

            _hideEntityList.Add(hideEntity);
        }

        _unitModelTransform = unitModelTransform;
    }

    public void Execute()
    {
        for(int idx = 0; idx < _hideEntityList.Count; idx++)
        {
            _hideEntityList[idx].Excecute();
        }

        CheckEnableHideEntity();
    }

    private void CheckEnableHideEntity()
    {
        float distance;
        float minDistance = 99999;
        int minDistanceIndex = -1;
        for(int idx = 0; idx < _hideEntityList.Count; idx++)
        {
            distance = (_unitModelTransform.localPosition - _hideEntityList[idx].gameObject.transform.localPosition).sqrMagnitude;

            if(minDistance > distance)
            {
                minDistance = distance;
                minDistanceIndex = idx;
            }
        }

        if(minDistance < ENABLE_HIDE_DISTANCE * ENABLE_HIDE_DISTANCE)
        {
            _enableHideEntity = _hideEntityList[minDistanceIndex];
        }
        else
        {
            _enableHideEntity = null;
        }
    }

    private const float ENABLE_HIDE_DISTANCE = 2.5f;

    [SerializeField]
    private GameObject _hideObj;
    [SerializeField]
    private List<Vector3> _hideEntityPosList = new List<Vector3>();

    private List<HideEntity> _hideEntityList = new List<HideEntity>();
    private Transform _unitModelTransform;
    private HideEntity _enableHideEntity;
}
