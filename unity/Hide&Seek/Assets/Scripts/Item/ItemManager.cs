using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void Initalize()
    {
        SetupItem();
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

        if(_itemPosList.Count == _getCount)
        {
            Debug.Log("FINISH ");
        }
    }

    [SerializeField]
    private GameObject _itemPrefab;
    [SerializeField]
    private List<Vector3> _itemPosList = new List<Vector3>();

    private int _getCount;
    private List<ItemEntity> _itemEntityList = new List<ItemEntity>();
}
