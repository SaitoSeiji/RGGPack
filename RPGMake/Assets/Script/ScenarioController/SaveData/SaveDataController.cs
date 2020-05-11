using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SaveDataController : SingletonMonoBehaviour<SaveDataController>
{
    [SerializeField] List<StaticDB> _staticDbList;
    [SerializeField] List<VariableDB> _variableDbList; 
    [SerializeField] Dictionary<string,List<SavedDBData>> _saveDataList = new Dictionary<string, List<SavedDBData>>();//<dbName,dataList>
    
    //セーブデータ用のデータを作成
    public void InitSaveDataList()
    {
        _saveDataList = new Dictionary<string, List<SavedDBData>>();
        foreach (var db in _variableDbList)
        {
            _saveDataList.Add(db.name, db.GetSavedDataList());
        }
    }
    string GetDBName<T>()
        where T : VariableDB
    {
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                return db.name;
            }
        }
        return null;
    }
    #region dataChenge

    public void SetData<T,K>(K data)
        where T:VariableDB
        where K: SavedDBData
    {
        var key= GetDBName<T>();
        List<SavedDBData> db = _saveDataList[key];
        for (int i = 0; i < db.Count; i++)
        {
            if (db[i]._serchId == data._serchId)
            {
                data.ModifyData();
                db[i] = data;
                break;
            }
        }
    }
    public List<T2> GetDB_var<T, T2>()
       where T : VariableDB
       where T2 : SavedDBData
    {
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                return _saveDataList[db.name].Select(x => (T2)x).ToList();
            }
        }
        return null;
    }
    public T GetDB_static<T>()
       where T : StaticDB
    {
        foreach (var db in _staticDbList)
        {
            if (db is T)
            {
                return db as T;
            }
        }
        return null;
    }
    #endregion
    #region IO
    public void SaveAction()
    {
        foreach(var data in _saveDataList)
        {
            JsonSaver.SaveAction<SavedDBData>(data.Value,data.Key);
        }
    }

    public void LoadAction()
    {
        foreach(var db in _variableDbList)
        {
            _saveDataList[db.name]= JsonSaver.LoadAction_list<SavedDBData>(db.name);
        }
    }
    #endregion
    #region test
    [ContextMenu("InitsaveTest")]
    void TestInitSave()
    {
        InitSaveDataList();
        SaveAction();
    }
    [ContextMenu("saveTest")]
    void TestSave()
    {
        SaveAction();
    }

    [ContextMenu("loadTest")]
    void TestLoad()
    {
        LoadAction();
    }

    [ContextMenu("setDataTest")]
    void TestSetData()
    {
        var db = GetDB_var<PlayerDB, SavedDBData_player>();
        var data = db[0];
        data._hpNow-=10;
        Debug.Log(data._hpNow);
        SetData<PlayerDB, SavedDBData_player>(data);
        var db2 = GetDB_var<PlayerDB, SavedDBData_player>();
        var data2 = db[0];
        Debug.Log(data2._hpNow);
    }
    #endregion
    #region editorOnly
    public void SetSaveDataList_editorOnly<T>(List<SavedDBData> list)
        where T : VariableDB
    {
        var key = GetDBName<T>();
        _saveDataList[key] = list;
    }
    public List<string> GetKeySet_editorOnly()
    {
        return _saveDataList.Keys.ToList();
    }
    #endregion
}
