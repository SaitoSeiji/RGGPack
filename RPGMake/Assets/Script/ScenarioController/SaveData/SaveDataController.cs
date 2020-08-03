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
        Debug.LogError($"db is not foung : type{typeof(T)}");
        return null;
    }
    #endregion
    #region セーブデータとmonobehaviourをつなぐもの
    public void Bridge_mono2data()
    {
        //パーティデータへの更新
        var target_party = GetDB_var<PartyDB, SavedDBData_party>()[0];
        target_party._PostionData = new SavedDBData_party.PostionData(Player.Instance.transform.position,Player.Instance._NowDirection,MapController.Instance._NowMapName);
    }

    public void Bridge_data2mono()
    {
        var target_party = GetDB_var<PartyDB, SavedDBData_party>()[0];
#if UNITY_EDITOR
        if (!GameContoller.Instance.coalFirstEvent_debug)
        {
            MapController.Instance.ChengeMap(target_party._PostionData._mapName,true,LoadCanvas.Instance.IsBlackNow);
            Player.Instance.SetPosition(target_party._PostionData._pos, target_party._PostionData._direction);
        }
#else
            MapController.Instance.ChengeMap(target_party._PostionData._mapName,true,LoadCanvas.Instance.IsBlackNow);
            Player.Instance.SetPosition(target_party._PostionData._pos, target_party._PostionData._direction);
#endif

    }
    #endregion
    #region IO
    public void SaveAction(bool actBridge=true)
    {
        if(actBridge)Bridge_mono2data();
        foreach(var data in _saveDataList)
        {
            JsonSaver.SaveAction<SavedDBData>(data.Value,data.Key);
        }
        Debug.Log("saved");
    }

    public void LoadAction(bool actBridge = true)
    {
        foreach(var db in _variableDbList)
        {
            _saveDataList[db.name]= JsonSaver.LoadAction_list<SavedDBData>(db.name);
        }
        if (actBridge) WaitAction.Instance.CoalWaitAction(()=> Bridge_data2mono(),0.5f);
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

    public void SetDB_editorOnly(List<StaticDB> db_static,List<VariableDB> db_variable)
    {
        _staticDbList = db_static;
        _variableDbList = db_variable;
    }
    #endregion
}
