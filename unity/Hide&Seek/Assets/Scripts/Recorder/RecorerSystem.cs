using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorerSystem : SingletonMonobehaviour<RecorerSystem>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void StartRecord(string recordKeyName)
    {
        if(_collection == null)
        {
            _collection = new RecordDataCollection();
        }

        _collection.Initialize(recordKeyName);
    }

    public void AddRecord(float gameTime, RecordData.RecordType type, float param1, float param2, float param3)
    {
        RecordData record = new RecordData();
        record.gameTime = gameTime;
        record.recordType = type;
        record.param1 = param1;
        record.param2 = param2;
        record.param3 = param3;

        _collection.recordList.Add(record);
    }

    public void SaveRecord()
    {
        string json = JsonUtility.ToJson(_collection);
        PlayerPrefs.SetString(_collection.keyName, json);
    }

    public void LoadRecord(string loadKeyName)
    {
        string json = PlayerPrefs.GetString(loadKeyName);
        _collection = JsonUtility.FromJson<RecordDataCollection>(json);
    }

    private RecordDataCollection _collection;
    private int _currentIndex;
}

public class RecordData
{
    public enum RecordType
    {
        GameStart,
        UnitMove,
        UnitAvoid,
        UnitHide,
        UnitHideEnd,
        GameEnd
    }

    public float gameTime;
    public RecordType recordType;
    public float param1;
    public float param2;
    public float param3;
}

public class RecordDataCollection
{
    public string keyName;
    public List<RecordData> recordList;

    public void Initialize(string recordKeyName)
    {
        keyName = recordKeyName;
        recordList = new List<RecordData>();
    }
}