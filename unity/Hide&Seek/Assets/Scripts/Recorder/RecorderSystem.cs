using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderSystem : SingletonMonobehaviour<RecorderSystem>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public bool isReplay;

    public RecordDataCollection collection{ get{ return _collection; } }

    /// <summary>
    /// リプレイ保存開始準備処理
    /// Prefsのキー・移動操作保存間隔を設定
    /// </summary>
    /// <param name="recordKeyName"></param>
    /// <param name="moveRecordInterval"></param>
    public void StartRecord(string recordKeyName, float moveRecordInterval)
    {
        if (_collection == null)
        {
            _collection = new RecordDataCollection();
        }

        _collection.moveRecordInterval = moveRecordInterval;
        _collection.Initialize(recordKeyName);
    }

    /// <summary>
    /// リプレイデータ追加
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="type"></param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    public void AddRecord(float gameTime, RecordData.RecordType type, float param1, float param2, float param3)
    {
        if (isReplay)
            return;

        RecordData record = new RecordData();
        record.gameTime = gameTime;
        record.recordType = type;
        record.param1 = param1;
        record.param2 = param2;
        record.param3 = param3;

        _collection.recordList.Add(record);
    }

    /// <summary>
    /// 追加されているリプレイデータを保存
    /// </summary>
    public void SaveRecord()
    {
        string json = JsonUtility.ToJson(_collection);
        PlayerPrefs.SetString(_collection.keyName, json);
    }

    /// <summary>
    /// 保存されているリプレイデータを読み込む
    /// </summary>
    /// <param name="loadKeyName"></param>
    /// <returns></returns>
    public RecordDataCollection LoadRecord(string loadKeyName)
    {
        _currentActionIndex = 0;
        _currentMoveInex = 0;

        string json = PlayerPrefs.GetString(loadKeyName);
        _collection = JsonUtility.FromJson<RecordDataCollection>(json);
        return _collection;
    }

    /// <summary>
    /// 次の移動操作リプレイデータを取得
    /// </summary>
    /// <returns></returns>
    public RecordData NextMoveRecord()
    {
        if (_currentMoveInex >= _collection.recordList.Count)
            return null;

        for (int idx = _currentMoveInex; idx < _collection.recordList.Count; idx++) 
        {
            if (_collection.recordList[idx].recordType != RecordData.RecordType.UnitMove)
                continue;

            _currentMoveInex = idx + 1;
            return _collection.recordList[idx];
        }

        return null;
    }

    /// <summary>
    /// 次の操作リプレイデータリストを取得
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    public List<RecordData> NextActionRecord(float gameTime)
    {
        List<RecordData> list = new List<RecordData>();

        if (_currentActionIndex >= _collection.recordList.Count)
            return null;

        for (int idx = _currentActionIndex; idx < _collection.recordList.Count; idx++)
        {
            if (_collection.recordList[idx].recordType == RecordData.RecordType.UnitMove)
                continue;

            if (gameTime < _collection.recordList[idx].gameTime)
            {
                break; ;
            }

            list.Add(_collection.recordList[idx]);
            _currentActionIndex = idx + 1;
        }

        return list;
    }

    private RecordDataCollection _collection;
    private int _currentActionIndex;
    private int _currentMoveInex;
}

[System.Serializable]
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

[System.Serializable]
public class RecordDataCollection
{
    public string keyName;
    public float moveRecordInterval;
    public List<RecordData> recordList;

    public void Initialize(string recordKeyName)
    {
        keyName = recordKeyName;
        recordList = new List<RecordData>();
    }
}