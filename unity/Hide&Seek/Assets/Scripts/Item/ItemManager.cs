using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int itemGetCount { get{ return _getCount; } }
    public int itemGetLimit{ get{ return _itemPosList.Count; } }

    public void Initalize(Action<int, int> getUIAction, Action<bool> finishAction)
    {
        SetupItem();

        _getUIAction = getUIAction;
        _getUIAction(_getCount, _itemPosList.Count);

        _finishAction = finishAction;
    }

    public void Execute()
    {
        for(int idx = 0; idx < _itemEntityList.Count; idx++)
        {
            _itemEntityList[idx].Excecute();
        }
    }

    private void SetupItem()
    {
        for (int idx = 0; idx < _itemPosList.Count; idx++)
        {
            GameObject obj = Instantiate(_itemPrefab, transform);

            obj.transform.localPosition = _itemPosList[idx];
            obj.transform.localRotation = Quaternion.Euler(new Vector3(50, 0, 0));

            var entity = obj.AddComponent<ItemEntity>();
            _itemEntityList.Add(entity);

            entity.Initalize(CountGetItem);
        }
    }

    private void CountGetItem()
    {
        _getCount++;

        _getUIAction(_getCount, _itemPosList.Count);

        if(_itemPosList.Count == _getCount)
        {
            Debug.Log("FINISH ");
            _finishAction(true);
        }
    }

    [SerializeField]
    private GameObject _itemPrefab;
    [SerializeField]
    private List<Vector3> _itemPosList = new List<Vector3>();

    private int _getCount;
    private List<ItemEntity> _itemEntityList = new List<ItemEntity>();
    private Action<int, int> _getUIAction;
    private Action<bool> _finishAction;
}
