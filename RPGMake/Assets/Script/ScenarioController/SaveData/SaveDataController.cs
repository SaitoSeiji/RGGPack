using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveDataController : SingletonMonoBehaviour<SaveDataController>
{
    [SerializeField] List<StaticDB> _staticDbList;
    [SerializeField] List<VariableDB> _variableDbList; 
    [SerializeField] List<DataMemberInspector> _memberSet=new List<DataMemberInspector>();
    [SerializeField] Dictionary<string,List<DBData>> _saveDataList = new Dictionary<string, List<DBData>>();

    void InitSaveDataList()
    {
        _saveDataList = new Dictionary<string, List<DBData>>();
        foreach (var db in _variableDbList)
        {
            var dataList = db.GetDataList();
            var tempSaveList = new List<DBData>();
            foreach (var data in dataList)
            {
                data.InitData();
                tempSaveList.Add(data._Data);
            }
            _saveDataList.Add(db.name, tempSaveList);
        }
    }

    public void InitStaticDataBase()
    {
        foreach (var db in _staticDbList)
        {
            var dataList = db.GetDataList();
            foreach (var data in dataList)
            {
                data.InitData();
            }
        }
    }

    void SetMemberSet()
    {
        _memberSet = new List<DataMemberInspector>();
        foreach (var dataList in _saveDataList)
        {
            foreach (var data in dataList.Value)
            {
                DataMemberInspector temp = null;
                foreach (var st in data._memberSet_int)
                {
                    temp = new DataMemberInspector(data._serchId);
                    temp.AddData(st.Key, st.Value,DataMemberInspector.HIKAKU.NONE);
                }
                _memberSet.Add(temp);
            }
        }
    }
    #region dataChenge
    public void SetData<T>(DataMemberInspector data)
        where T : AbstractDB
    {
        foreach(var d in data._memberSet)
        {
            SetData<T>(data._id, d.memberName, d.data);
            //Debug.Log(d.data+":"+d.memberName);
        }
        SetMemberSet();
    }
    public void SetData<T>(string id,string memberName,int data)
        where T:AbstractDB
    {
        string name = "";
        AbstractDB targetDB = null;
        foreach(var db in _variableDbList)
        {
            if(db is T)
            {
                name = db.name;
                targetDB = db;
                break;
            }
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("SaveDataController:SetData:name is null");
            return;
        }

        foreach(var saveData in _saveDataList)
        {
            if(saveData.Key==name)
            {
                foreach(var unit in saveData.Value)
                {
                    if (unit._serchId == id)
                    {
                        if (unit._memberSet_int.ContainsKey(memberName))
                        {
                            unit._memberSet_int[memberName] = data;
                            targetDB.DataUpdateAction(unit);
                        }
                        else
                        {
                            Debug.Log("SaveDataController:SetData:memberName is uncorrect");
                        }
                        return;
                    }
                }
                Debug.Log("SaveDataController:SetData:id is uncorrect");
                return;
            }
        }
    }
    public string GetText<T>(string id, string memberName)
        where T:AbstractDB
    {
        string name = "";
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                name = db.name;
                break;
            }
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("SaveDataController:SetData:name is null");
            return "";
        }

        foreach (var saveData in _saveDataList)
        {
            if (saveData.Key == name)
            {
                foreach (var unit in saveData.Value)
                {
                    if (unit._serchId == id)
                    {
                        if (unit._memberSet_st.ContainsKey(memberName))
                        {
                            return unit._memberSet_st[memberName];
                        }
                        else
                        {
                            Debug.Log("SaveDataController:SetData:memberName is uncorrect:"+memberName);
                        }
                        return "";
                    }
                }
                Debug.Log("SaveDataController:SetData:id is uncorrect");
                return "";
            }
        }
        return "";
    }
    public int GetData<T>(DataMemberInspector data)
        where T : AbstractDB
    {
        foreach(var mem in data._memberSet)
        {
            var result= GetData<T>(data._id, mem.memberName);
            //Debug.Log(result+":"+mem.memberName);
            return result;
        }
        return -1;
    }
    public int GetData<T>(string id, string memberName)
        where T : AbstractDB
    {
        string name = "";
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                name = db.name;
                break;
            }
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("SaveDataController:SetData:name is null");
            return -1;
        }

        foreach (var saveData in _saveDataList)
        {
            if (saveData.Key == name)
            {
                foreach (var unit in saveData.Value)
                {
                    if (unit._serchId == id)
                    {
                        if (unit._memberSet_int.ContainsKey(memberName))
                        {
                            return unit._memberSet_int[memberName];
                        }
                        else
                        {
                            Debug.Log("SaveDataController:SetData:memberName is uncorrect");
                        }
                        return -1;
                    }
                }
                Debug.Log("SaveDataController:SetData:id is uncorrect");
                return -1;
            }
        }
        return -1;
    }

    public List<DBData> GetDB_var<T>()
        where T : VariableDB
    {
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                return _saveDataList[db.name];
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
    public void SaveAction()
    {
        foreach(var data in _saveDataList)
        {
            JsonSaver.SaveAction<DBData>(data.Value,data.Key);
        }
    }

    public void LoadAction()
    {
        foreach(var db in _variableDbList)
        {
            _saveDataList[db.name]= JsonSaver.LoadAction_list<DBData>(db.name);
        }
        SetMemberSet();
    }

    [ContextMenu("InitsaveTest")]
    public void TestInitSave()
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

    [ContextMenu("valueChengeTest")]
    void TestValueChenge()
    {
        SetData<FlagDB>("t1", "flagNum", 5);
        SetMemberSet();
    }
}
