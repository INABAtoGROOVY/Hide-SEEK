using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitManager : MonoBehaviour
{
    public void Initialize(Transform unitModelTransform)
    {
        for (int idx = 0; idx < _enemyUnitPosList.Count; idx++)
        {
            EnemyUnit enemy = Instantiate(_enemyUnitObj, transform).GetComponent<EnemyUnit>();

            enemy.transform.localPosition = _enemyUnitPosList[idx];
            enemy.transform.localRotation = Quaternion.identity;

            enemy.Initialize(_wayPointObj);
            _enemyUnitList.Add(enemy);
        }

        _unitModelTransform = unitModelTransform;
    }

    public void Execute()
    {
        for (int idx = 0; idx < _enemyUnitList.Count; idx++)
        {
            _enemyUnitList[idx].Execute();
        }

        EnemyDistanceToSoundConvert();
    }

    private void EnemyDistanceToSoundConvert()
    {
        float distance;
        float minDistance = 99999;
        for (int idx = 0; idx < _enemyUnitList.Count; idx++)
        {
            distance = (_unitModelTransform.localPosition - _enemyUnitList[idx].gameObject.transform.localPosition).sqrMagnitude;

            if (minDistance > distance)
            {
                minDistance = distance;
            }
        }

        SoundManager.Instance.eightBitConverter.SetRatio(DistanceNormalization(minDistance));
    }

    private float DistanceNormalization(float distance)
    {
        float ret;
        if(distance > CONVERT_DISTANCE_MAX)
        {
            distance = CONVERT_DISTANCE_MAX;
        }

        if(distance < CONVERT_DISTANCE_MIN)
        {
            distance = CONVERT_DISTANCE_MIN;
        }

        ret = (distance - CONVERT_DISTANCE_MIN) / (CONVERT_DISTANCE_MAX - CONVERT_DISTANCE_MIN);
        ret = (ret - 1.0f) * -1;
        return ret;
    }

    [SerializeField]
    private GameObject _enemyUnitObj;
    [SerializeField]
    private List<Vector3> _enemyUnitPosList = new List<Vector3>();
    [SerializeField]
    private GameObject _wayPointObj;

    private const float CONVERT_DISTANCE_MAX = 2000.0f;
    private const float CONVERT_DISTANCE_MIN = 10.0f;

    private List<EnemyUnit> _enemyUnitList = new List<EnemyUnit>();
    private Transform _unitModelTransform;
}
